using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Appets.BusinessLogic.Interface;
using Appets.DataAccess.Interface;
using Appets.Domain;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Appets.WebApi.Models;
using Appets.Exceptions;
using Appets.WebApi.Filters;

namespace Appets.WebApi.Controllers
{
    [ApiController]
    [Route("pets")]
    public class PetController : ControllerBase
    {
        private IPetLogic LogicService;
        public PetController(IPetLogic service) : base()
        {
            this.LogicService = service;
        }

        //[ServiceFilter(typeof(AuthenticationFilter))]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get()
        {
            try
            {
                List<Pet> listPets = this.LogicService.GetAll().ToList(); ;
                List<PetOut> listPetsOut = listPets.ConvertAll(a => new PetOut(a));
                return Ok(listPetsOut);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        //[ServiceFilter(typeof(AuthenticationFilter))]
        [HttpPut("PetFound/{id}", Name = "PetFound")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PetFound(Guid id)
        {
            try
            {
                return Ok(new PetOut(this.LogicService.ChangePetStatusToFound(id)));
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [ServiceFilter(typeof(AuthenticationFilter))]
        [HttpGet("{id}", Name = "GetPet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetById(Guid id)
        {
            try
            {
                return Ok(new PetOut(this.LogicService.Get(id)));
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

       // [ServiceFilter(typeof(AuthenticationFilter))]
        [HttpGet("owner/{userId}", Name = "GetPetsByUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetPetsByUser(Guid userId)
        {
            try
            {
                List<Pet> listPets = this.LogicService.GetPetsByUser(userId).ToList();
                List<PetOut> listPetsOut = listPets.ConvertAll(a => new PetOut(a));
                return Ok(listPetsOut);
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

        //[ServiceFilter(typeof(AuthenticationFilter))]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Put(Guid id, [FromBody] PetIn petIn)

        {
            try
            {
                petIn.Id = id;
                return Ok(new PetOut(this.LogicService.Update(id, petIn.ToEntity())));
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

        [HttpPost("{idUser}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromBody] PetIn petIn, Guid idUser)

        {
            try
            {
                Pet createdPet = this.LogicService.CreateWithUser(petIn.ToEntity(), idUser);
                return CreatedAtRoute("GetPet", new { id = createdPet.Id }, createdPet.Id);
            }
            catch (BadArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DBKeyNotFoundException)
            {
                return BadRequest("No User with that id");
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        //[ServiceFilter(typeof(AuthenticationFilter))]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(Guid id)
        {
            try
            {
                this.LogicService.Remove(id);
                return Ok(id);
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
