using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using connections.Data;
using connections.DTO;
using connections.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace connections.Controllers
{
    [Route("api/[Controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthenticationController(IAuthRepository authRepo, IConfiguration config, IMapper mapper )
        {
            _config = config;
            _authRepo = authRepo;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterDtos Userdto)
        {
           // throw new Exception("take it register");
            Userdto.Username = Userdto.Username.ToLower();
            if (await _authRepo.ExistingUser(Userdto.Username))
                return BadRequest("Username is already taken");

            var UserModel = _mapper.Map<User>(Userdto);
            var createUser = await _authRepo.Register(UserModel, Userdto.Password);
            var usertoreturn = _mapper.Map<UserDTO>(createUser);
            return CreatedAtRoute("getUser", new {controller = "User", id = createUser.Id},usertoreturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] loginDTO logindto)
        {
            var userResponse = await _authRepo.Login(logindto.Username, logindto.Password);
            if (userResponse == null)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                       new Claim(ClaimTypes.NameIdentifier,userResponse.Id.ToString()),
                        new Claim(ClaimTypes.Name,userResponse.Username)
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            var user = _mapper.Map<UserListDTO>(userResponse);
            return Ok(new { tokenString , user });
        }
    }
}