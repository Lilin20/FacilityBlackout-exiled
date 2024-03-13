using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Collections.Generic;

namespace FacilityBlackout
{
    public class RoomManager
    {
        public static List<Room> RoomsInZone(ZoneType Zone)
        {
            List<Room> Rooms = new();
            foreach (Room Room in Room.List)
            {
                if (Room.Zone == Zone)
                {
                    Rooms.Add(Room);
                }
            }
            return Rooms;
        }

        public static List<Room> GetRandomRoomsInZone(ZoneType zone, int count)
        {
            List<Room> roomsInZone = RoomsInZone(zone);
            List<Room> randomRooms = new List<Room>();

            if (roomsInZone.Count == 0)
            {
                return randomRooms;
            }

            Random random = new Random();
            for (int i = 0; i < count; i++)
            {
                int randomIndex = random.Next(0, roomsInZone.Count);
                randomRooms.Add(roomsInZone[randomIndex]);
            }

            return randomRooms;
        }
    }
}
