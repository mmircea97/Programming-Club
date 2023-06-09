﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammingClub.Helpers;
using ProgrammingClub.Models;
using ProgrammingClub.Models.CreateModels;
using ProgrammingClub.Services;
using System.Net;

namespace ProgrammingClub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventTypeController : Controller
    {
        private readonly IEventTypeService _eventTypeService;

        public EventTypeController(IEventTypeService eventType)
        {
            _eventTypeService = eventType;
        }

        [HttpGet]
        public async Task<IActionResult> GetEventType()
        {
            try
            {
                var eventType = await _eventTypeService.GetEventTypesAsync();
                if (eventType != null && eventType.Count() > 0)
                    return Ok(eventType);

                return StatusCode((int)HttpStatusCode.NoContent, ErrorMessagesEnum.NoElementFound);
            }
            catch (Exception ex) { return StatusCode((int)HttpStatusCode.InternalServerError, ex); }

        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetEventTypeById([FromRoute] Guid id)
        {
            try
            {
                EventType? eventType = await _eventTypeService.GetEventTypeByIdAsync(id);

                if (eventType != null)

                    return Ok(eventType);



                return StatusCode((int)HttpStatusCode.NoContent, ErrorMessagesEnum.NoElementFound);

            }

            catch (Exception ex) { return StatusCode((int)HttpStatusCode.InternalServerError, ex); }


        }

        [HttpPost]
        public async Task<IActionResult> PostEventType([FromBody] CreateEventType eventType)
        {
            try
            {
                if (eventType != null)
                {
                    await _eventTypeService.CreateEventTypeAsync(eventType);
                    return Ok(SuccessMessegesEnum.ElementSuccesfullyAdded);
                }
                return StatusCode((int)HttpStatusCode.BadRequest);
            }
            catch (Exception ex) { return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember([FromRoute] Guid id)
        {


            try
            {
                var result = await _eventTypeService.DeleteEventTypeAsync(id);
                if (result)
                    return Ok(Helpers.SuccessMessegesEnum.ElementSuccesfullyDeleted);

            }
            catch (Exception ex) { return StatusCode((int)HttpStatusCode.InternalServerError, ex); }
            return StatusCode((int)HttpStatusCode.BadRequest, "No elem found");

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember([FromRoute] Guid id, [FromBody] EventType eventType)
        {
            try
            {
                if (eventType == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest);
                }

                var updatedEventType = await _eventTypeService.UpdateEventTypeAsync(id, eventType);
                if (updatedEventType == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, ErrorMessagesEnum.NoElementFound);
                }

                return Ok(SuccessMessegesEnum.ElementSuccesfullyUpdated);
            }
            catch (Exception ex) { return StatusCode((int)HttpStatusCode.InternalServerError, ex); }

        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchEventType([FromRoute] Guid id, [FromBody] EventType eventType)
        {
            try
            {
                if (eventType == null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest);
                }

                var updatedEventType = await _eventTypeService.UpdateEventTypePartiallyAsync(id, eventType);
                if (updatedEventType == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, ErrorMessagesEnum.NoElementFound);
                }

                return Ok(SuccessMessegesEnum.ElementSuccesfullyUpdated);
            }
            catch (Exception ex) { return StatusCode((int)HttpStatusCode.InternalServerError, ex); }
        }
    }
}
