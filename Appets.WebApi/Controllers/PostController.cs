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
using Appets.Domain.Enums;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace Appets.WebApi.Controllers
{
    [ApiController]
    [Route("posts")]
    public class PostController : ControllerBase
    {
        private IPostLogic PostLogicService;
        private IPetLogic PetLogicService;
        private IConfiguration Configuration;
        public PostController(IPetLogic petLogicService, IPostLogic postLogicService, IConfiguration configuration) : base()
        {
            this.PostLogicService = postLogicService;
            this.PetLogicService = petLogicService;
            this.Configuration = configuration;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Get([FromQuery(Name = "range")] int range, [FromQuery(Name = "ubication")] string ubication)
        {
            try
            {
                List<Post> listPosts = this.PostLogicService.GetAll().ToList();
                if (range > 0 && ubication != null)
                {
                    listPosts = this.PostLogicService.FilterPostsByRange(listPosts, range, ubication);
                }
                List<PostOut> listPostsOut = listPosts.ConvertAll(a => new PostOut(a));
                return Ok(listPostsOut);
            }
            catch (BadArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}", Name = "GetPost")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetById(Guid id)
        {
            try
            {
                return Ok(new PostOut(this.PostLogicService.Get(id)));
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

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Put(Guid id, [FromBody] PostIn postIn)

        {
            try
            {
                postIn.Id = id;
                return Ok(new PostOut(this.PostLogicService.Update(id, postIn.ToEntity())));
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
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromBody] PostIn postIn)
        {
            try
            {
                postIn.Type = PostTypeEnum.SEEN;
                Post createdPost = this.PostLogicService.Create(postIn.ToEntity());
                return CreatedAtRoute("GetPost", new { id = createdPost.Id }, createdPost.Id);
            }
            catch (BadArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [ServiceFilter(typeof(AuthenticationFilter))]
        [HttpPost("{idMascota}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromBody] PostIn postIn, Guid idMascota)
        {
            try
            {
                postIn.Type = PostTypeEnum.LOST;
                Post createdPost = this.PostLogicService.Create(postIn.ToEntity());
                this.PetLogicService.AddPost(idMascota, createdPost);

                // The topic name can be optionally prefixed with "/topics/".
                var topic = "/topics/lostPet";

                // See documentation on defining a message payload.
                var message = new NotificationMessage()
                {
                    To = topic,
                    Notification = new Notification() { Title = "Alerta de mascota perdida", Text = createdPost.Title },
                };

                // Send a message to the devices subscribed to the provided topic.
                SendNotification(message);

                return CreatedAtRoute("GetPost", new { id = createdPost.Id }, createdPost.Id);
            }
            catch (BadArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch (DBKeyNotFoundException e)
            {
                return BadRequest("No Pet with that id");
            }
            catch (Exception e)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        private static async Task SendNotification(NotificationMessage notification)
        {
            string serverKey = "AAAAZrkwUeQ:APA91bH4BIIvZnbKKnQiaDrEWTom-948HRiASXWbEcUw-XHSc7cx9CHYTdVqxAGkKqNstc3r--OeunTaa5nho39h1YVHRX5dVOVkhdFyRAq1BUV38PBKEc874C5UtHwJVJbanSH3UN1w";
            string api = "https://fcm.googleapis.com/fcm/send";
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, api);

                string jsonMessage = JsonConvert.SerializeObject(notification, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });


                request.Headers.TryAddWithoutValidation("Authorization", "key=" + serverKey);
                request.Content = new StringContent(jsonMessage, Encoding.UTF8, "application/json");

                HttpResponseMessage result;
                using (var client = new HttpClient())
                {
                    result = await client.SendAsync(request);
                    bool status = result.IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {

            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(Guid id)
        {
            try
            {
                this.PostLogicService.Remove(id);
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

        [HttpGet("tagAnimals")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetTagAnimals()
        {
            try
            {
                List<string> animals = this.Configuration.GetSection("TagAnimals").Get<List<string>>();
                return Ok(animals);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("tagBreeds")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetTagBreeds([FromQuery(Name = "animal")] string animal)
        {
            try
            {
                List<string> breeds = this.Configuration.GetSection("TagBreeds:" + animal).Get<List<string>>();
                return Ok(breeds);
            }
            catch (Exception)
            {
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{postId}/similarPosts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetSimilarPosts(Guid postId)
        {
            try
            {
                Dictionary<string, int> scores = Configuration
                .GetSection("TagScores")
                .Get<Dictionary<string, int>>();

                int limitSimilarPost = this.Configuration.GetSection("LimitSimilarPost").Get<int>();

                List<Tuple<int, Post>> listPosts = this.PostLogicService.GetSimilarPosts(postId, scores, limitSimilarPost).ToList();
                List<SimilarPostOut> listPostsOut = listPosts.ConvertAll(a => new SimilarPostOut(a));
                return Ok(listPostsOut);
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

        [HttpPut("{idPost}/ignoreSimilar/{IdIgnoredPost}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult IgnoreSimilar(Guid idPost, Guid IdIgnoredPost)

        {
            try
            {
                return Ok(new PostOut(this.PostLogicService.IgnoreSimilar(idPost, IdIgnoredPost)));
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
