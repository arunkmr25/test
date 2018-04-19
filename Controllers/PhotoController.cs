using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using connections.Data.connectionRep;
using connections.DTO;
using connections.Helpers;
using connections.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace connections.Controllers
{
    [Authorize]
    [Route("api/user/{userid}/photos")]
    public class PhotoController : Controller
    {
        private readonly IConnection _Conrepo;
        private readonly IMapper _Mapper;
        private readonly IOptions<CloudinarySettings> _Cloudconfig;
        private readonly Cloudinary _Cloudinary;

        public PhotoController(IConnection conrepo, IMapper mapper, IOptions<CloudinarySettings> cloudconfig)
        {
            _Conrepo = conrepo;
            _Mapper = mapper;
            _Cloudconfig = cloudconfig;
            Account acc = new Account(
                _Cloudconfig.Value.CloudName,
                _Cloudconfig.Value.ApiKey,
                _Cloudconfig.Value.ApiSecret
            );

            _Cloudinary = new Cloudinary(acc);
        }
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetSinglePhoto(int id)
        {

            var PhotoFromRepo = _Conrepo.GetSinglePhotoDetails(id);

            var photos = _Mapper.Map<PhotoForRetreival>(PhotoFromRepo);

            return Ok(photos);
        }

        [HttpPost]

        public async Task<ActionResult> AddPhoto(int userid, PhotoForCreationDto photoDto)
        {
            var user = await _Conrepo.GetUser(userid);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var currentuserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (currentuserId != user.Id)
            {
                return Unauthorized();
            }
            var file = photoDto.File;

            var imageuploadresult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var imageupladparams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    imageuploadresult = _Cloudinary.Upload(imageupladparams);
                }
            }
            photoDto.Url = imageuploadresult.Uri.ToString();
            photoDto.PublicId = imageuploadresult.PublicId;

            var photo = _Mapper.Map<Photo>(photoDto);

            photo.User = user;

            if (!user.Photos.Any(x => x.IsMain))
            {
                photo.IsMain = true;
            }
            user.Photos.Add(photo);
            if (await _Conrepo.SaveAll())
            {
                var phototoReturn = _Mapper.Map<PhotoForRetreival>(photo);
                return CreatedAtRoute("GetPhoto", new { id = photo.Id }, phototoReturn);
            }
            return BadRequest("Photo could not be created");
        }

          [HttpPost("{id}/setmain")]
        public async Task<ActionResult> SetMainPhoto(int userid , int id){
            if(userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var PhotoFromRepo =await _Conrepo.GetSinglePhotoDetails(id);
            if(PhotoFromRepo == null)
                return NotFound();

            if(PhotoFromRepo.IsMain)
                return BadRequest("Selected photo is already main");
            
            var currentMainPhoto = await _Conrepo.getMainPhoto(userid);
            if(currentMainPhoto != null)
            currentMainPhoto.IsMain =false;

            PhotoFromRepo.IsMain= true;

            if(await _Conrepo.SaveAll()){
                return NoContent();
            }
            return BadRequest("Photo not set as main");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePhoto(int userid ,int id){
            if(userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var PhotoFromRepo =await _Conrepo.GetSinglePhotoDetails(id);
            if(PhotoFromRepo == null)
                return NotFound();

            if(PhotoFromRepo.IsMain)
                return BadRequest("Photo cannot be deleted as its your profile picture");

            _Conrepo.Delete(PhotoFromRepo);
            if(PhotoFromRepo.PublicId !=null){
                var deleteparams = new DeletionParams(PhotoFromRepo.PublicId);
                var result = _Cloudinary.Destroy(deleteparams);
                if(result.Result == "ok"){
                    _Conrepo.Delete(PhotoFromRepo);
                }
            }

            if(await _Conrepo.SaveAll()){
                return Ok();
            }

            return BadRequest("Photo not deleted");           
        }
    }
}