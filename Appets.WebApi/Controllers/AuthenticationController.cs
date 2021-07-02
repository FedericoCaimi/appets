using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Appets.BusinessLogic.Interface;
using Appets.Domain;
using Microsoft.AspNetCore.Http;
using Appets.WebApi.Filters;
using System.Linq;
using Appets.WebApi.Models;
using Appets.Exceptions;

namespace IMMRequest.WebApi.Controllers
{
    [ApiController]
    [Route("authentication")]
    public class AuthenticationController : ControllerBase
    {
        private IAuthenticationLogic LogicService;
        public AuthenticationController(IAuthenticationLogic service) : base()
        {
            this.LogicService = service;
        }

        [ServiceFilter(typeof(AuthenticationFilter))]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get()
        {
            try
            {
                List<Session> listSessions = this.LogicService.GetAll().ToList(); ;
                List<AuthenticationOut> listSessionsOut = listSessions.ConvertAll(a => new AuthenticationOut(a));
                return Ok(listSessionsOut);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }


        [ServiceFilter(typeof(AuthenticationFilter))]
        [HttpGet("{id}", Name = "GetSession")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetById(Guid id)
        {
            try
            {
                return Ok(new AuthenticationOut(this.LogicService.Get(id)));
            }
            catch (BadArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromBody] AuthenticationIn user)
        {
            try
            {
                Session createdSession = this.LogicService.Login(user.ToEntity());
                return CreatedAtRoute("GetSession", new { id = createdSession.Id }, new AuthenticationOut(createdSession));
            }
            catch (BadArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (BadLoginException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpDelete("{token}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(Guid token)
        {
            try
            {
                this.LogicService.Logout(token);
                return Ok(token);
            }
            catch (BadArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
