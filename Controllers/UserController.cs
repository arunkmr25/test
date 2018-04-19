using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using connections.Data.connectionRep;
using connections.DTO;
using connections.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[Controller]")]
    public class UserController : Controller
    {
        private readonly IConnection _connection;
        private readonly IMapper _mapper;
        public UserController(IConnection connection, IMapper mapper)
        {
            _mapper = mapper;
            _connection = connection;

        }
        [HttpGet]
        public async Task<ActionResult> GetAllUsers(UserParams userParam)
        {
            var currentUser= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var repositoryUser = await _connection.GetUser(currentUser);
            userParam.UserId = currentUser;
            if(string.IsNullOrEmpty(userParam.Gender)){
                userParam.Gender = repositoryUser.Gender == "male" ? "female" : "male"; 
            }
            var users = await _connection.GetUsers(userParam);
            var clientresponse = _mapper.Map<IEnumerable<UserListDTO>>(users);
            Response.AddPagination(users.CurrentPage , users.PageSize , users.TotalPages , users.TotalCount);
            return Ok(clientresponse);
        }
        [HttpGet("{id}", Name="getUser")]
        public async Task<ActionResult> GetUser(int id)
        {
            var user = await _connection.GetUser(id);
            var clientresponse = _mapper.Map<UserDTO>(user);
            return Ok(clientresponse);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] UserUpdateDTO updateInfo){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }
            var currentUser= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var repositoryUser = await _connection.GetUser(id);

            if(repositoryUser == null){
                return NotFound($"the user with {id} is not available");
            }
            if(currentUser != repositoryUser.Id){
                return Unauthorized();
            }

            _mapper.Map(updateInfo,repositoryUser);

            if(await _connection.SaveAll()){
                return NoContent();
            }

            throw new Exception ($"User updation Failed for {id}");
        }
    }
}