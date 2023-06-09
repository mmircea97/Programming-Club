﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProgrammingClub.DataContext;
using ProgrammingClub.Exceptions;
using ProgrammingClub.Models;
using ProgrammingClub.Models.CreateModels;
using System.Text.RegularExpressions;

namespace ProgrammingClub.Services
{
    public class EventsService : IEventsService
    {
        private readonly ProgrammingClubDataContext _context;
        private readonly IMapper _mapper;
        private IModeratorService _moderatorService;
        private IEventTypeService _eventTypeService;

        public EventsService(ProgrammingClubDataContext context, IMapper mapper, IModeratorService moderatorService, IEventTypeService eventTypeService)
        {
            _context = context;
            _mapper = mapper;
            _moderatorService = moderatorService;
            _eventTypeService = eventTypeService;
        }

        public async Task CreateEventAsync(CreateEvent eventCreate)
        {  
            var newEvent = _mapper.Map<Event>(eventCreate);
            await ValidateEvent(newEvent);
            newEvent.IdEvent = Guid.NewGuid();
            _context.Entry(newEvent).State = EntityState.Added;
            await _context.SaveChangesAsync();
            
        }

        public async Task<bool> DeleteEventAsync(Guid id)
        {
            if(!await EventExistByIdAsync(id)) 
                return false;

            _context.Events.Remove(new Event { IdEvent = id });
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Event>> GetEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event?> UpdatePartiallyEventAsync(Guid id, Event eventUpdate)
        {
            bool eventIsChanged = false, idModeratorIsChanged = false, idEventTypeIsChanged = false;
            var eventFromDatabase = await GetEventByIdAsync(id);
            eventUpdate.IdEvent = id;

            if (eventFromDatabase == null)
            {
                return null;
            }

            if(!string.IsNullOrEmpty(eventUpdate.Name) && eventUpdate.Name != eventFromDatabase.Name)
            {
                eventFromDatabase.Name = eventUpdate.Name;
                eventIsChanged = true;
            }
            if (!string.IsNullOrEmpty(eventUpdate.Description) && eventUpdate.Description != eventFromDatabase.Name)
            {
                eventFromDatabase.Description = eventUpdate.Description;
                eventIsChanged = true;
            }
            if(eventUpdate.IdModerator.HasValue && eventUpdate.IdModerator != eventFromDatabase.IdModerator)
            {
                eventFromDatabase.IdModerator = eventUpdate.IdModerator;
                eventIsChanged = true;
                idModeratorIsChanged = true;
            }
            if (eventUpdate.IdEventType.HasValue && eventUpdate.IdEventType != eventFromDatabase.IdEventType)
            {
                eventFromDatabase.IdEventType = eventUpdate.IdEventType;
                eventIsChanged = true;
                idEventTypeIsChanged = true;
            }
            if(!eventIsChanged)
            {
                throw new ModelValidationException(Helpers.ErrorMessagesEnum.ZeroUpdatesToSave);
            }
            if (idModeratorIsChanged || idEventTypeIsChanged)
            {
                await ValidateEvent(eventFromDatabase);
            }

            _context.Update(eventFromDatabase);
            await _context.SaveChangesAsync();
            return eventFromDatabase;

        }

        public async Task<Event?> UpdateEventAsync(Guid id, CreateEvent eventUpdate)
        {
            if(!await EventExistByIdAsync(id))
            {
                return null;
            }
            

            var eventUpdated = _mapper.Map<Event>(eventUpdate);
            eventUpdated.IdEvent = id;
            await ValidateEvent(eventUpdated);

            eventUpdate.IdEvent = id;
            _context.Update(eventUpdated);
            await _context.SaveChangesAsync();
            return eventUpdated;
        }

        public async Task<Event?> GetEventByIdAsync(Guid id)
        {
            return await _context.Events.FirstOrDefaultAsync(e => e.IdEvent == id);
        }

        public async Task<bool> EventExistByIdAsync(Guid? id)
        {
            if (!id.HasValue)
            {
                return false;
            }

            return await _context.Events.AnyAsync(e => e.IdEvent == id);
        }

        private async Task ValidateEvent(Event validateEvent)
        {
            Guid? idModerator = validateEvent.IdModerator;
            Guid? idEventType = validateEvent.IdEventType;

            if (!await _moderatorService.ModeratorExistByIdAsync(idModerator))
            {
                throw new ModelValidationException(Helpers.ErrorMessagesEnum.Moderator.NoModeratorFound);
            }
            if (!await _eventTypeService.EventTypeExistsByIdAsync(idEventType))
            {
                throw new ModelValidationException(Helpers.ErrorMessagesEnum.EventType.NoEventTypeFound);
            }
        }
    }
}
