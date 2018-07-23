using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyAdminAPI.Models;

namespace PropertyAdminAPI {

    #region PROJECT
    [Route ("api/[Controller]")]
    public class ProjectsController : Controller {

        // POST api/admin/projects
        [HttpPost]
        // [Route ("api/[Controller]")]
        public async Task<IActionResult> Post ([FromBody] MultiPartProject multiPartProject) {
            // public async Task<IActionResult> Post ([FromBody] Project2 project) {
            try {

                // var a = new DateTime(multiPartProject.project.release_date.Year, multiPartProject.project.release_date.Month,
                // multiPartProject.project.release_date.Day+1 );
                using (var _context = new PropertyContext ()) {
                    var newProject = new Project {
                    project_name = multiPartProject.project.project_name,
                    location = multiPartProject.project.location,
                    available_unit = multiPartProject.project.available_unit,
                    price = multiPartProject.project.price,

                    // no need validation, always filled as a new data
                    image_src = new Utils ().SaveImageFile (multiPartProject),

                    developer_id = multiPartProject.project.developer_id,
                    is_new_project = multiPartProject.project.is_new_project,
                    is_publish = multiPartProject.project.is_publish,
                    release_date = multiPartProject.project.release_date.AddDays (1), // error receive value of day date

                    is_deleted = false,
                    created_date = DateTime.Now,
                    created_by = "developer",
                    };

                    // add one or more units
                    if (multiPartProject.MultiPartUnits != null) {
                        if (multiPartProject.MultiPartUnits.Count () > 0) {
                            var newUnits = new List<Unit> ();
                            foreach (MultiPartUnit _multiPartUnit in multiPartProject.MultiPartUnits) {
                                var newUnit = new Unit {
                                    // project = newProject,
                                    idx = _multiPartUnit.unit.idx,
                                    unit_name = _multiPartUnit.unit.unit_name,
                                    price = _multiPartUnit.unit.price,
                                    image_src = new Utils ().SaveImageFile (_multiPartUnit),
                                    surface_area = _multiPartUnit.unit.surface_area,
                                    building_area = _multiPartUnit.unit.building_area,
                                    bed_room = _multiPartUnit.unit.bed_room,
                                    car_port = _multiPartUnit.unit.car_port,
                                    spec_content = _multiPartUnit.unit.spec_content,

                                    is_deleted = false,
                                    created_date = DateTime.Now,
                                    created_by = "developer"
                                };

                                if (_multiPartUnit.MultiPartGaleryPhoto != null) {
                                    if (_multiPartUnit.MultiPartGaleryPhoto.Count > 0) {
                                        var newSlidesPhoto = new List<GaleryPhoto> ();
                                        foreach (var slide in _multiPartUnit.MultiPartGaleryPhoto) {
                                            newSlidesPhoto.Add (new GaleryPhoto {
                                                unit = _multiPartUnit.unit,
                                                    idx = slide.galeryPhoto.idx,
                                                    caption = slide.galeryPhoto.caption,
                                                    is_video = false,
                                                    image_src = new Utils ().SaveImageFile (slide),
                                                    is_deleted = false,
                                                    created_date = DateTime.Now,
                                                    created_by = "developer"
                                            });
                                        }
                                        newUnit.galery_photos = newSlidesPhoto;
                                    }
                                }

                                if (_multiPartUnit.MultiPartGaleryVideo != null) {
                                    if (_multiPartUnit.MultiPartGaleryVideo.Count > 0) {
                                        var newSlidesVideo = new List<GaleryVideo> ();
                                        foreach (var slide in _multiPartUnit.MultiPartGaleryVideo) {
                                            newSlidesVideo.Add (new GaleryVideo {
                                                idx = slide.galeryVideo.idx,
                                                    caption = slide.galeryVideo.caption,
                                                    is_video = true,
                                                    image_src = slide.galeryVideo.image_src,
                                                    is_deleted = false,
                                                    created_date = DateTime.Now,
                                                    created_by = "developer"
                                            });
                                        }
                                        newUnit.galery_videos = newSlidesVideo;
                                    }
                                }

                                if (_multiPartUnit.MultiPartGaleryFloorPlan != null) {
                                    if (_multiPartUnit.MultiPartGaleryFloorPlan.Count > 0) {
                                        var newSlidesFloorPlan = new List<GaleryFloorPlan> ();
                                        foreach (var slide in _multiPartUnit.MultiPartGaleryFloorPlan) {
                                            newSlidesFloorPlan.Add (new GaleryFloorPlan {
                                                idx = slide.galeryFloorPlan.idx,
                                                    caption = slide.galeryFloorPlan.caption,
                                                    is_video = false,
                                                    image_src = new Utils ().SaveImageFile (slide),
                                                    is_deleted = false,
                                                    created_date = DateTime.Now,
                                                    created_by = "developer"
                                            });
                                        }
                                        newUnit.galery_floor_plan = newSlidesFloorPlan;
                                    }
                                }

                                if (_multiPartUnit.MultiPartGaleryProgress != null) {
                                    if (_multiPartUnit.MultiPartGaleryProgress.Count > 0) {
                                        var newSlidesProgress = new List<GaleryProgress> ();
                                        foreach (var slide in _multiPartUnit.MultiPartGaleryProgress) {
                                            newSlidesProgress.Add (new GaleryProgress {
                                                idx = slide.galeryProgress.idx,
                                                    caption = slide.galeryProgress.caption,
                                                    is_video = false,
                                                    image_src = new Utils ().SaveImageFile (slide),
                                                    is_deleted = false,
                                                    created_date = DateTime.Now,
                                                    created_by = "developer"
                                            });
                                        }
                                        newUnit.galery_progress = newSlidesProgress;
                                    }
                                }
                                newUnits.Add (newUnit);
                            }
                            newProject.units = newUnits;
                        }
                    }

                    if (multiPartProject.MultiPartSlideshow != null) {
                        var slideshow  = new List<Slideshow> ();
                        foreach (var slide in multiPartProject.MultiPartSlideshow)
                        {
                            var _slideshow = new Slideshow {
                                // project = newProject,
                                idx = slide.slide.idx,
                                caption = slide.slide.caption,
                                is_video = false,
                                image_src = new Utils ().SaveImageFile (slide),

                                is_deleted = false,
                                created_date = DateTime.Now,
                                created_by = "developer"
                            };
                            slideshow.Add(_slideshow);
                        }
                        newProject.slideshow = slideshow;

                    }
                    await _context.Projects.AddAsync (newProject);
                    await _context.SaveChangesAsync ();
                    return Ok ();
                }

            } catch (System.Exception e) {
                return BadRequest (e.Message.ToString ());
            }
        }

        // PUT api/project
        [HttpPut]
        public async Task<IActionResult> Put ([FromBody] MultiPartProject multiPartProject) {
            try {
                using (var _context = new PropertyContext ()) {

                    var editProject = _context.Projects
                        .Include (c => c.developer)
                        .Include (c => c.units).ThenInclude (c => c.galery_photos)
                        .Include (c => c.units).ThenInclude (c => c.galery_videos)
                        .Include (c => c.units).ThenInclude (c => c.galery_floor_plan)
                        .Include (c => c.units).ThenInclude (c => c.galery_progress)
                        .SingleOrDefault (c => c.id == multiPartProject.project.id);

                    editProject.project_name = multiPartProject.project.project_name;
                    editProject.location = multiPartProject.project.location;
                    editProject.available_unit = multiPartProject.project.available_unit;
                    editProject.price = multiPartProject.project.price;

                    if (multiPartProject.project.image_src != editProject.image_src) {
                        editProject.image_src = new Utils ().SaveImageFile (multiPartProject);
                    }

                    editProject.developer_id = multiPartProject.project.developer_id;
                    editProject.is_new_project = multiPartProject.project.is_new_project;
                    editProject.is_publish = multiPartProject.project.is_publish;
                    editProject.release_date = multiPartProject.project.release_date.AddDays (1);

                    editProject.modified_date = DateTime.Now;
                    editProject.modified_by = "creator";

                    if (multiPartProject.MultiPartUnits.Count == 0) {
                        // delete all existing units
                        var isUnitExist = _context.Units.Where (c => c.project_id.Equals (editProject.id)).ToList ();
                        if (isUnitExist.Count > 0) {
                            IList<Unit> unitsToRemove = new List<Unit> ();
                            foreach (var unit in isUnitExist)
                                unitsToRemove.Add (unit);
                            _context.Units.RemoveRange (unitsToRemove);
                        }
                        // RemoveUnits(multiPartProject.MultiPartUnits);
                    } else {

                        // check removed unit...
                        if (editProject.units.Count > 0) {
                            IList<Unit> unitsToRemove = new List<Unit> ();
                            foreach (var unit in editProject.units) {
                                var isUnitExist = multiPartProject.MultiPartUnits.SingleOrDefault (c => c.unit.id.Equals (unit.id));
                                if (isUnitExist == null) {
                                    unitsToRemove.Add (unit);
                                    _context.Units.RemoveRange (unitsToRemove);
                                }
                            }
                        }

                        // save new or edited unit
                        foreach (var _multiPartUnit in multiPartProject.MultiPartUnits) {

                            // search existing unit 
                            var editUnit = editProject.units.SingleOrDefault (c => c.id == _multiPartUnit.unit.id);
                            if (editUnit != null) {
                                // update existing unit
                                editUnit.idx = _multiPartUnit.unit.idx;
                                editUnit.unit_name = _multiPartUnit.unit.unit_name;
                                editUnit.price = _multiPartUnit.unit.price;

                                if (_multiPartUnit.unit.image_src != editUnit.image_src)
                                    editUnit.image_src = new Utils ().SaveImageFile (_multiPartUnit);

                                editUnit.surface_area = _multiPartUnit.unit.surface_area;
                                editUnit.building_area = _multiPartUnit.unit.building_area;
                                editUnit.bed_room = _multiPartUnit.unit.bed_room;
                                editUnit.car_port = _multiPartUnit.unit.car_port;
                                editUnit.spec_content = _multiPartUnit.unit.spec_content;
                                editUnit.modified_date = DateTime.Now;
                                editUnit.modified_by = "developer";

                                // save new & update galeries
                                if (_multiPartUnit.MultiPartGaleryPhoto == null) {
                                    // delete all existing
                                    IList<GaleryPhoto> galeriesToRemove = new List<GaleryPhoto> ();
                                    var isGaleryExist = _context.GaleryPhotos.Where (c => c.unit_id.Equals (editUnit.id)).ToList ();
                                    foreach (var galery in isGaleryExist)
                                        galeriesToRemove.Add (galery);
                                    _context.GaleryPhotos.RemoveRange (galeriesToRemove);
                                } else {
                                    // check removed galery, delete some of galery
                                    if (editUnit.galery_photos.Count > 0) {
                                        IList<GaleryPhoto> galeryToRemove = new List<GaleryPhoto> ();
                                        foreach (var galery in editUnit.galery_photos) {
                                            var isGaleryExist = _multiPartUnit.MultiPartGaleryPhoto.SingleOrDefault (c => c.galeryPhoto.id.Equals (galery.id));
                                            if (isGaleryExist == null) {
                                                galeryToRemove.Add (galery);
                                            }
                                        }
                                        _context.GaleryPhotos.RemoveRange (galeryToRemove);
                                    }

                                    foreach (var _multiPartGaleryPhoto in _multiPartUnit.MultiPartGaleryPhoto) {
                                        //search existing galery
                                        var editPhoto = editUnit.galery_photos.SingleOrDefault (c => c.id == _multiPartGaleryPhoto.galeryPhoto.id);
                                        if (editPhoto != null) {
                                            // update 
                                            editPhoto.idx = _multiPartGaleryPhoto.galeryPhoto.idx;
                                            editPhoto.caption = _multiPartGaleryPhoto.galeryPhoto.caption;

                                            if (_multiPartGaleryPhoto.galeryPhoto.image_src != editPhoto.image_src)
                                                editPhoto.image_src = new Utils ().SaveImageFile (_multiPartGaleryPhoto);

                                            editPhoto.modified_date = DateTime.Now;
                                            editPhoto.modified_by = "developer";
                                        } else {
                                            // new 
                                            var newSlidePhoto = new GaleryPhoto {
                                                idx = _multiPartGaleryPhoto.galeryPhoto.idx,
                                                caption = _multiPartGaleryPhoto.galeryPhoto.caption,
                                                is_video = false,
                                                image_src = new Utils ().SaveImageFile (_multiPartGaleryPhoto),
                                                is_deleted = false,
                                                created_date = DateTime.Now,
                                                created_by = "developer"
                                            };
                                            editUnit.galery_photos.Add (newSlidePhoto);
                                        }
                                    }
                                }

                                if (_multiPartUnit.MultiPartGaleryFloorPlan == null) {
                                    IList<GaleryFloorPlan> galeriesToRemove = new List<GaleryFloorPlan> ();
                                    var isGaleryExist = _context.GaleryFloorPlan.Where (c => c.unit_id.Equals (editUnit.id)).ToList ();
                                    foreach (var galery in isGaleryExist)
                                        galeriesToRemove.Add (galery);
                                    _context.GaleryFloorPlan.RemoveRange (galeriesToRemove);

                                } else {
                                    if (editUnit.galery_floor_plan.Count > 0) {
                                        IList<GaleryFloorPlan> galeryToRemove = new List<GaleryFloorPlan> ();
                                        foreach (var galery in editUnit.galery_floor_plan) {
                                            var isGaleryExist = _multiPartUnit.MultiPartGaleryFloorPlan.SingleOrDefault (c => c.galeryFloorPlan.id.Equals (galery.id));
                                            if (isGaleryExist == null) {
                                                galeryToRemove.Add (galery);
                                            }
                                        }
                                        _context.GaleryFloorPlan.RemoveRange (galeryToRemove);
                                    }

                                    foreach (var _multiPartGaleryFloorPlan in _multiPartUnit.MultiPartGaleryFloorPlan) {
                                        //search existing galery
                                        var editFloorPlan = editUnit.galery_floor_plan.SingleOrDefault (c => c.id == _multiPartGaleryFloorPlan.galeryFloorPlan.id);

                                        if (editFloorPlan != null) {
                                            // update 
                                            editFloorPlan.idx = _multiPartGaleryFloorPlan.galeryFloorPlan.idx;
                                            editFloorPlan.caption = _multiPartGaleryFloorPlan.galeryFloorPlan.caption;

                                            if (_multiPartGaleryFloorPlan.galeryFloorPlan.image_src != editFloorPlan.image_src)
                                                editFloorPlan.image_src = new Utils ().SaveImageFile (_multiPartGaleryFloorPlan);

                                            editFloorPlan.modified_date = DateTime.Now;
                                            editFloorPlan.modified_by = "developer";
                                        } else {
                                            // new 
                                            var newSlideFloorPlan = new GaleryFloorPlan {
                                                idx = _multiPartGaleryFloorPlan.galeryFloorPlan.idx,
                                                caption = _multiPartGaleryFloorPlan.galeryFloorPlan.caption,
                                                is_video = false,
                                                image_src = new Utils ().SaveImageFile (_multiPartGaleryFloorPlan),
                                                is_deleted = false,
                                                created_date = DateTime.Now,
                                                created_by = "developer"
                                            };
                                            editUnit.galery_floor_plan.Add (newSlideFloorPlan);
                                        }
                                    }
                                    // }
                                }

                                if (_multiPartUnit.MultiPartGaleryProgress == null) {
                                    IList<GaleryProgress> galeriesToRemove = new List<GaleryProgress> ();
                                    var isGaleryExist = _context.GaleryProgress.Where (c => c.unit_id.Equals (editUnit.id)).ToList ();
                                    foreach (var galery in isGaleryExist)
                                        galeriesToRemove.Add (galery);
                                    _context.GaleryProgress.RemoveRange (galeriesToRemove);
                                } else {

                                    if (editUnit.galery_progress.Count > 0) {
                                        IList<GaleryProgress> galeryToRemove = new List<GaleryProgress> ();
                                        foreach (var galery in editUnit.galery_progress) {
                                            var isGaleryExist = _multiPartUnit.MultiPartGaleryProgress.SingleOrDefault (c => c.galeryProgress.id.Equals (galery.id));
                                            if (isGaleryExist == null) {
                                                galeryToRemove.Add (galery);
                                            }
                                        }
                                        _context.GaleryProgress.RemoveRange (galeryToRemove);
                                    }

                                    foreach (var _multiPartGaleryProgress in _multiPartUnit.MultiPartGaleryProgress) {
                                        //search existing galery
                                        var editProgress = editUnit.galery_progress.SingleOrDefault (c => c.id == _multiPartGaleryProgress.galeryProgress.id);
                                        if (editProgress != null) {
                                            // update 
                                            editProgress.idx = _multiPartGaleryProgress.galeryProgress.idx;
                                            editProgress.caption = _multiPartGaleryProgress.galeryProgress.caption;

                                            if (_multiPartGaleryProgress.galeryProgress.image_src != editProgress.image_src)
                                                editProgress.image_src = new Utils ().SaveImageFile (_multiPartGaleryProgress);

                                            editProgress.modified_date = DateTime.Now;
                                            editProgress.modified_by = "developer";
                                        } else {
                                            // new 
                                            var newSlideProgress = new GaleryProgress {
                                                idx = _multiPartGaleryProgress.galeryProgress.idx,
                                                caption = _multiPartGaleryProgress.galeryProgress.caption,
                                                is_video = false,
                                                image_src = new Utils ().SaveImageFile (_multiPartGaleryProgress),
                                                is_deleted = false,
                                                created_date = DateTime.Now,
                                                created_by = "developer"
                                            };
                                            editUnit.galery_progress.Add (newSlideProgress);
                                        }
                                    }
                                }

                                if (_multiPartUnit.MultiPartGaleryVideo == null) {
                                    IList<GaleryVideo> galeriesToRemove = new List<GaleryVideo> ();
                                    var isGaleryExist = _context.GaleryVideos.Where (c => c.unit_id.Equals (editUnit.id)).ToList ();
                                    foreach (var galery in isGaleryExist)
                                        galeriesToRemove.Add (galery);
                                    _context.GaleryVideos.RemoveRange (galeriesToRemove);

                                } else {

                                    if (editUnit.galery_videos.Count > 0) {
                                        IList<GaleryVideo> galeryToRemove = new List<GaleryVideo> ();
                                        foreach (var galery in editUnit.galery_videos) {
                                            var isGaleryExist = _multiPartUnit.MultiPartGaleryVideo.SingleOrDefault (c => c.galeryVideo.id.Equals (galery.id));
                                            if (isGaleryExist == null) {
                                                galeryToRemove.Add (galery);
                                            }
                                        }
                                        _context.GaleryVideos.RemoveRange (galeryToRemove);
                                    }

                                    foreach (var _multiPartGaleryVideo in _multiPartUnit.MultiPartGaleryVideo) {
                                        //search existing galery
                                        var editVideo = editUnit.galery_videos.SingleOrDefault (c => c.id == _multiPartGaleryVideo.galeryVideo.id);
                                        if (editVideo != null) {
                                            // update 
                                            editVideo.idx = _multiPartGaleryVideo.galeryVideo.idx;
                                            editVideo.caption = _multiPartGaleryVideo.galeryVideo.caption;

                                            if (_multiPartGaleryVideo.galeryVideo.image_src != editVideo.image_src)
                                                editVideo.image_src = _multiPartGaleryVideo.galeryVideo.image_src;

                                            editVideo.modified_date = DateTime.Now;
                                            editVideo.modified_by = "developer";
                                        } else {
                                            // new 
                                            var newSlideVideo = new GaleryVideo {
                                                idx = _multiPartGaleryVideo.galeryVideo.idx,
                                                caption = _multiPartGaleryVideo.galeryVideo.caption,
                                                is_video = true,
                                                image_src = _multiPartGaleryVideo.galeryVideo.image_src,
                                                is_deleted = false,
                                                created_date = DateTime.Now,
                                                created_by = "developer"
                                            };
                                            editUnit.galery_videos.Add (newSlideVideo);
                                        }
                                    }
                                }

                            } else {
                                // save new unit  
                                var newUnit = new Unit {
                                    idx = _multiPartUnit.unit.idx,
                                    unit_name = _multiPartUnit.unit.unit_name,
                                    price = _multiPartUnit.unit.price,
                                    image_src = new Utils ().SaveImageFile (_multiPartUnit),
                                    surface_area = _multiPartUnit.unit.surface_area,
                                    building_area = _multiPartUnit.unit.building_area,
                                    bed_room = _multiPartUnit.unit.bed_room,
                                    car_port = _multiPartUnit.unit.car_port,
                                    spec_content = _multiPartUnit.unit.spec_content,

                                    is_deleted = false,
                                    created_date = DateTime.Now,
                                    created_by = "developer"
                                };

                                //save new galeries
                                if (_multiPartUnit.MultiPartGaleryPhoto != null) {
                                    IList<GaleryPhoto> galeryToAdd = new List<GaleryPhoto> ();
                                    foreach (var _multiPartGaleryPhoto in _multiPartUnit.MultiPartGaleryPhoto) {
                                        //new  galery
                                        var newSlide = new GaleryPhoto {
                                            idx = _multiPartGaleryPhoto.galeryPhoto.idx,
                                            caption = _multiPartGaleryPhoto.galeryPhoto.caption,
                                            is_video = false,
                                            image_src = new Utils ().SaveImageFile (_multiPartGaleryPhoto),
                                            is_deleted = false,
                                            created_date = DateTime.Now,
                                            created_by = "developer"
                                        };
                                        galeryToAdd.Add (newSlide);
                                    }
                                    newUnit.galery_photos = galeryToAdd;
                                }

                                if (_multiPartUnit.MultiPartGaleryFloorPlan != null) {
                                    IList<GaleryFloorPlan> galeryToAdd = new List<GaleryFloorPlan> ();
                                    foreach (var _multiPartGaleryFloorPlan in _multiPartUnit.MultiPartGaleryFloorPlan) {
                                        var newSlideFloorPlan = new GaleryFloorPlan {
                                            idx = _multiPartGaleryFloorPlan.galeryFloorPlan.idx,
                                            caption = _multiPartGaleryFloorPlan.galeryFloorPlan.caption,
                                            is_video = false,
                                            image_src = new Utils ().SaveImageFile (_multiPartGaleryFloorPlan),
                                            is_deleted = false,
                                            created_date = DateTime.Now,
                                            created_by = "developer"
                                        };
                                        galeryToAdd.Add (newSlideFloorPlan);
                                    }
                                    newUnit.galery_floor_plan = galeryToAdd;
                                }

                                if (_multiPartUnit.MultiPartGaleryProgress != null) {
                                    IList<GaleryProgress> galeryToAdd = new List<GaleryProgress> ();
                                    foreach (var _multiPartGaleryProgress in _multiPartUnit.MultiPartGaleryProgress) {
                                        var newSlideProgress = new GaleryProgress {
                                            idx = _multiPartGaleryProgress.galeryProgress.idx,
                                            caption = _multiPartGaleryProgress.galeryProgress.caption,
                                            is_video = false,
                                            image_src = new Utils ().SaveImageFile (_multiPartGaleryProgress),
                                            is_deleted = false,
                                            created_date = DateTime.Now,
                                            created_by = "developer"
                                        };
                                        galeryToAdd.Add (newSlideProgress);
                                    }
                                    newUnit.galery_progress = galeryToAdd;
                                }

                                if (_multiPartUnit.MultiPartGaleryVideo != null) {
                                    IList<GaleryVideo> galeryToAdd = new List<GaleryVideo> ();
                                    foreach (var _multiPartGaleryVideo in _multiPartUnit.MultiPartGaleryVideo) {
                                        var newSlideVideo = new GaleryVideo {
                                            idx = _multiPartGaleryVideo.galeryVideo.idx,
                                            caption = _multiPartGaleryVideo.galeryVideo.caption,
                                            is_video = true,
                                            image_src = _multiPartGaleryVideo.galeryVideo.image_src,
                                            is_deleted = false,
                                            created_date = DateTime.Now,
                                            created_by = "developer"
                                        };
                                        galeryToAdd.Add (newSlideVideo);
                                    }
                                    newUnit.galery_videos = galeryToAdd;
                                }
                                editProject.units.Add (newUnit);
                            }
                        }

                    }

                    await _context.SaveChangesAsync ();
                    return Ok ();
                }
            } catch (System.Exception e) {
                return BadRequest (e.Message.ToString ());
            }
        }

        // GET api/projects
        [HttpGet]
        public async Task<List<Project>> Get () {
            using (var _context = new PropertyContext ()) {
                return await _context.Projects
                    .Include (p => p.developer)
                    .Where (c => c.is_deleted == false)
                    .OrderByDescending (c => c.release_date).ToListAsync ();

                /*return await _context.Projects
                    .Include (d => d.developer)
                    .Include (u => u.Units).ThenInclude (c => c.galery_photos)
                    .Include (u => u.Units).ThenInclude (c => c.galery_denah)
                    .Include (u => u.Units).ThenInclude (c => c.galery_video)
                    .Include (u => u.Units).ThenInclude (c => c.galery_progress)
                    .Where (c => c.is_deleted == false)
                    // .Single(p => p.id == id);
                    .ToListAsync ();*/
            }
        }

        // GET api/projects/getdetail/2
        [HttpGet ("getdetail/{id}")]
        public IActionResult GetDetail (int id) {
            try {
                // https://code.i-harness.com/en/q/97a80c
                using (var _context = new PropertyContext ()) {

                    var result = _context.Projects
                        // .Include (d => d.developer)
                        .Where (c => c.id == id)
                        // .Where (c => c.is_deleted == false)
                        // .Include (u => u.Units) //.ThenInclude(c => c.galery_photos)
                        .Select (p => new {
                            id = p.id,
                                project_name = p.project_name,
                                location = p.location,
                                available_unit = p.available_unit,
                                price = p.price,
                                image_src = p.image_src,
                                is_new_project = p.is_new_project,
                                is_publish = p.is_publish,
                                release_date = p.release_date,
                                developer_id = p.developer_id,
                                
                                slideshow = p.slideshow.Select( s => new {
                                    id = s.id,
                                    idx = s.idx,
                                    caption = s.caption,
                                    image_src = s.image_src
                                }).OrderBy ( slideIdx => slideIdx.idx).ToList(),

                                units = p.units.Select (u => new {
                                    id = u.id,
                                        idx = u.idx,
                                        unit_name = u.unit_name,
                                        price = u.price,
                                        image_src = u.image_src,
                                        surface_area = u.surface_area,
                                        building_area = u.building_area,
                                        bed_room = u.bed_room,
                                        bath_room = u.bath_room,
                                        car_port = u.car_port,
                                        spec_content = u.spec_content,
                                        project_id = u.project_id,
                                        is_deleted = u.is_deleted,

                                        

                                        galery_photos = u.galery_photos.Select (photo => new {
                                            id = photo.id,
                                                idx = photo.idx,
                                                caption = photo.caption,
                                                image_src = photo.image_src
                                        }).OrderBy (gIdx => gIdx.idx).ToList (), //photo. ) OrderBy (c => c.idx).ToList (),

                                        galery_floor_plan = u.galery_floor_plan.Select (floor => new {
                                            id = floor.id,
                                                idx = floor.idx,
                                                caption = floor.caption,
                                                image_src = floor.image_src
                                        }).OrderBy (gIdx => gIdx.idx).ToList (),

                                        galery_progress = u.galery_progress.Select (progress => new {
                                            id = progress.id,
                                                idx = progress.idx,
                                                caption = progress.caption,
                                                image_src = progress.image_src
                                        }).OrderBy (gIdx => gIdx.idx).ToList (),

                                        galery_videos = u.galery_videos.Select (video => new {
                                            id = video.id,
                                                idx = video.idx,
                                                caption = video.caption,
                                                image_src = video.image_src
                                        }).OrderBy (gIdx => gIdx.idx).ToList (),

                                })
                                .Where (c => c.is_deleted == false)
                                .OrderBy (c => c.idx).ToList (),

                        }).SingleOrDefault ();
                    //. OrderBy(idx => idx.Units. ) //.ThenInclude (c => c.galery_photos)
                    // .Include (u => u.Units).ThenInclude (c => c.galery_denah)
                    // .Include (u => u.Units).ThenInclude (c => c.galery_video)
                    // .Include (u => u.Units).ThenInclude (c => c.galery_progress)

                    // .Where (c => c.is_deleted == false)
                    // .OrderBy(x => x.Units)
                    // .Single (p => p.id == id);

                    // var aa = result.Units.OrderByDescending (c => c.idx);

                    return Ok (result);
                    // return new OkResult();
                }

            } catch (System.Exception) {
                throw;
            }
        }

        // DELETE api/project/5
        [HttpDelete ("{id}")]
        public async Task<IActionResult> Delete (int id) {
            try {
                using (var _context = new PropertyContext ()) {
                    var data = _context.Projects.Single (c => c.id == id);

                    data.is_deleted = true;
                    // _context.Remove(removeData);
                    await _context.SaveChangesAsync ();
                    return new OkResult ();
                }
            } catch (System.Exception e) {
                return new NotFoundResult ();
            }
        }
    }

    #endregion

    #region UNIT
    [Route ("api/[Controller]")]
    public class UnitController : Controller {

        // public IEnumerable<User> Get () {
        //     return GetAll ();
        // }

        // private IEnumerable<User> GetAll () {

        //     // var users == null;// = _context.Users;
        //     using (var db = new PropertyContext ()) {
        //         // db.Developers.Add (new Developer { developer_name = "developer1" });
        //         // db.SaveChanges ();

        //         return _context.Users.ToList ();

        //     }

        //     // return users;//new string[] { "value1", "value2", "value3" };
        // }

    }
    #endregion

    #region STAFF
    [Route ("api/[Controller]")]
    public class UserController : Controller {

        // private readonly PropertyContext _context;

        [HttpGet]
        public IEnumerable<Staff> Get () {
            return GetAll ();
        }

        private IEnumerable<Staff> GetAll () {

            using (var db = new PropertyContext ()) {
                return db.Staffs.ToList ();
            }

            // return new string[] { "value1", "value2", "value3" };
        }

        // POST api/user
        [HttpPost]
        public IActionResult Post ([FromBody] Staff staff) {

            using (var db = new PropertyContext ()) {
                db.Staffs.Add (staff);
                db.SaveChanges ();
            }

            return Ok (); // (item);
        }
    }
    #endregion

    #region DEVELOPER

    [Route ("api/[Controller]")]
    public class DevelopersController : Controller {

        // GET api/developers
        [HttpGet]
        public async Task<List<Developer>> Get () {
            try {
                using (var _context = new PropertyContext ()) {
                    return await _context.Developers
                        .Where (c => c.is_deleted == false)
                        .OrderBy (c => c.name).ToListAsync ();
                }

            } catch (System.Exception) {

                throw;
            }
        }

        // POST api/developers
        [HttpPost]
        public async Task<IActionResult> Post ([FromBody] MultiPartDeveloper multiPartDeveloper) {
            try {
                using (var _context = new PropertyContext ()) {

                    var newData = new Developer {
                    name = multiPartDeveloper.developer.name,
                    email = multiPartDeveloper.developer.email,
                    phone = multiPartDeveloper.developer.phone,
                    website = multiPartDeveloper.developer.website,
                    image_src = new Utils ().SaveImageFile (multiPartDeveloper),
                    is_deleted = false,

                    created_date = DateTime.Now,
                    created_by = "creator"
                    };

                    _context.Developers.Add (newData);
                    await _context.SaveChangesAsync ();
                    return new OkResult (); // Ok (newData);
                }
            } catch (System.Exception e) {
                return NotFound (e.ToString ());
            }
        }

        // PUT api/values/developers
        [HttpPut]
        public async Task<IActionResult> Put ([FromBody] MultiPartDeveloper multiPartDeveloper) {
            try {
                using (var _context = new PropertyContext ()) {
                    var editProject = _context.Developers.Single (c => c.id == multiPartDeveloper.developer.id);
                    editProject.name = multiPartDeveloper.developer.name;
                    editProject.email = multiPartDeveloper.developer.email;
                    editProject.phone = multiPartDeveloper.developer.phone;
                    editProject.website = multiPartDeveloper.developer.website;

                    if (multiPartDeveloper.imageAttr != null)
                        editProject.image_src = new Utils ().SaveImageFile (multiPartDeveloper);
                    editProject.is_deleted = false;

                    editProject.modified_date = DateTime.Now;
                    editProject.modified_by = "modificator";

                    await _context.SaveChangesAsync ();
                    return new OkResult (); // Ok (newData);
                }
            } catch (System.Exception e) {
                return NotFound (e.ToString ());
            }
            // return new OkResult ();
        }

        // DELETE api/developers/5
        [HttpDelete ("{id}")]
        public async Task<IActionResult> Delete (int id) {
            try {
                using (var _context = new PropertyContext ()) {
                    var data = _context.Developers.Single (c => c.id == id);

                    data.is_deleted = true;
                    // _context.Remove(removeData);
                    await _context.SaveChangesAsync ();
                    return new OkResult ();
                }
            } catch (System.Exception e) {
                return new NotFoundResult ();
            }
        }

    }

    #endregion

    #region COMMON

    [Route ("api/[Controller]")]
    public class UploadController : Controller {

        // public byte[] imageToByteArray(System.Drawing.Image imageIn)
        // {
        //  MemoryStream ms = new MemoryStream();
        //  imageIn.Save(ms,System.Drawing.Imaging.ImageFormat.Gif);
        //  return  ms.ToArray();
        // }

        private readonly IHostingEnvironment _environment;
        // public ImageController (IHostingEnvironment environment) {
        //     _environment = environment ??
        //         throw new ArgumentNullException (nameof (environment));
        // }

        [HttpPost ("UploadFiles")]
        public async Task<IActionResult> UploadFiles (List<IFormFile> files) {
            long size = files.Sum (f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName ();

            foreach (var formFile in files) {
                if (formFile.Length > 0) {
                    using (var stream = new FileStream (filePath, FileMode.Create)) {
                        await formFile.CopyToAsync (stream);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok (new { count = files.Count, size, filePath });

        }

        [HttpPost ("UploadFile")]
        public async Task<IActionResult> UploadFile (IFormFile file) {

            var tempe = new Utils ().SaveImageFile (file);
            if (file == null) throw new Exception ("File is null");

            if (file.Length == 0) throw new Exception ("File is empty");

            // var uploads = Path.GetTempFileName (); //Path.Combine(_environment.WebRootPath, "uploads");
            // var filePath = Path.GetTempFileName ();
            // var filePath = new Utils ().GenerateFileName (file);
            if (file.Length > 0) {
                // using (var fileStream = new FileStream (Path.Combine (uploads, file.FileName), FileMode.Create)) {
                // using (var fileStream = new FileStream (filePath, FileMode.Create)) {
                //     await file.CopyToAsync (fileStream);
                // }
            }
            // var a = new Utils ().GenerateFileName (file);
            return Ok (); // OkResult (); // Ok (new { count = files.Count, size, filePath });
            // var b = a.GenerateFileName();
        }
    }

    public class Utils {

        public void UploadFile (IFormFile file) {

            var tempe = new Utils ().SaveImageFile (file);
            if (file == null) throw new Exception ("File is null");

            if (file.Length == 0) throw new Exception ("File is empty");

            // var uploads = Path.GetTempFileName (); //Path.Combine(_environment.WebRootPath, "uploads");
            // var filePath = Path.GetTempFileName ();
            // var filePath = new Utils ().GenerateFileName (file);
            if (file.Length > 0) {
                // using (var fileStream = new FileStream (Path.Combine (uploads, file.FileName), FileMode.Create)) {
                // using (var fileStream = new FileStream (filePath, FileMode.Create)) {
                //     await file.CopyToAsync (fileStream);
                // }
            }
            // var a = new Utils ().GenerateFileName (file);
            // return Ok (); // OkResult (); // Ok (new { count = files.Count, size, filePath });
            // var b = a.GenerateFileName();
        }
        public Utils () { }

        public string SaveImageFile (object dataSource) {
            string result = "";

            try {
                var uploadPath = Path.Combine (Directory.GetCurrentDirectory (), "wwwroot\\assets\\uploads\\");
                if (!Directory.Exists (uploadPath)) Directory.CreateDirectory (uploadPath);

                if ((dataSource as MultiPart).imageAttr != null) {
                    result = string.Format ("{0}{1}{2}", uploadPath, DateTime.Now.ToString ("yyyy.MM.dd-HH.mm.ss.ffff"), (dataSource as MultiPart).imageAttr.extension);
                    File.WriteAllBytes (result, (dataSource as MultiPart).imageAttr.byteArr);
                }
                const string removeString = "\\assets";
                int index = result.IndexOf (removeString);
                result = result.Remove (0, index).Replace ("\\", "/").Remove (0, 1);

            } catch (System.Exception ex) {

                throw;
            }
            return result;
        }

    }

    #endregion

}