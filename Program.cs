using InboundEntryConsole.Models;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace OutboundEntryConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new QueueSystemDBContext())
            {

                var query = from map in context.MappingDetails
                            where map.IsProcessed == false
                            orderby map.PublisherId, map.ConsumerId, map.CreatedDate ascending
                            select map;

                var consumerId = 0;
                var publisherID = 0;

                foreach (var q in query)
                {
                    if (consumerId != q.ConsumerId && publisherID!= q.PublisherId)
                    {
                        context.InboundEntry.Add(new InboundEntry
                        {
                            MappingId = q.MappingId,
                            CreatedDate = DateTime.Now
                        }
                        );

                        consumerId = q.ConsumerId;
                        publisherID = q.PublisherId;
                    }
                }
                context.SaveChanges();
            }
        }
    }
}
