﻿using Guide_Me.DTO;
using Guide_Me.Migrations;
using Guide_Me.Models;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Guide_Me.Services
{
    public class PlaceService: IPlaceService
    {
        private readonly ApplicationDbContext _context;
        public PlaceService(ApplicationDbContext context) {
            _context = context;
        }

        public List<PlaceItemDto> GetPlaceItems(string placeName)
        {
            var place = _context.Places.FirstOrDefault(p => p.PlaceName == placeName);
            var placeItemsMap = new List<PlaceItemDto>();

            if (place != null)
            {
  
                var placeItems=_context.placeItem
                    .Include(p => p.PlaceItemMedias)
                    .Where(pi => pi.placeID==place.Id)
                    .ToList();

                foreach (var placeItem in placeItems)
                {
                    PlaceWithoutMediaDto placeDto = new PlaceWithoutMediaDto
                        {
                            Name = place.PlaceName,
                            Category = place.Category,
                      
                        };

                    var placeItemDto = new PlaceItemDto
                    {
                         ID=placeItem.ID,
                         placeItemName=placeItem.placeItemName,
                         Media =placeItem.PlaceItemMedias != null?
                         placeItem.PlaceItemMedias.Select(media=>new ItemMediaDto
                         {
                             MediaContent=media.MediaContent,
                             MediaType=media.MediaType,
                         })
                         .ToList() 
                         : new List<ItemMediaDto>()

                    };

                    placeItemsMap.Add(placeItemDto);
                }
            }

            return placeItemsMap;
        }


        public List<PlaceDto> GetPlaces(string cityName) {
            
            var city = _context.Cities.FirstOrDefault(c => c.CityName == cityName);
            if (city == null)
            {    
                
                return null;
            }

            var places = _context.Places
                     .Include(p => p.PlaceMedias) 
                     .Where(p => p.CityId == city.Id)
                     .ToList();


            List<PlaceDto> placeDtos = new List<PlaceDto>();

            
            foreach (var place in places)
            {
                PlaceDto placeDto = new PlaceDto
                {
                    Name = place.PlaceName,
                    Category = place.Category,
                    Media = place.PlaceMedias != null
                    ? place.PlaceMedias.Select(m => new PlaceMediaDto
                        {
                            MediaContent = m.MediaContent
                        }).ToList()
                    : new List<PlaceMediaDto>()
                };
                placeDtos.Add(placeDto);
            }
            return placeDtos;
        }
      

    }
}
