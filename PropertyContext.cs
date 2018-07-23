/*
dotnet ef migrations add InitialCreate
dotnet ef database update
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace PropertyAdminAPI.Models {
    public class PropertyContext : DbContext {
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Slideshow> Slideshow { get; set; }
        // public DbSet<Slide> Slides { get; set; }
        public DbSet<GaleryPhoto> GaleryPhotos { get; set; }
        public DbSet<GaleryVideo> GaleryVideos { get; set; }
        public DbSet<GaleryProgress> GaleryProgress { get; set; }
        public DbSet<GaleryFloorPlan> GaleryFloorPlan { get; set; }

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder) {
            // optionsBuilder.U UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=MyDatabase;Trusted_Connection=True;");
            optionsBuilder.UseMySql ("server=localhost;database=property-web-engine;user=root;password=openme; SslMode=none");
            // optionsBuilder.UseSqlite("Data Source=property-engine.db");
        }

        // protected override void OnModelCreating (ModelBuilder modelBuilder) {
            // modelBuilder.Entity<GaleryPhoto> ().HasBaseType<Slide> ();
            // modelBuilder.Entity<GaleryVideo> ().HasBaseType<Slide> ();

            // modelBuilder.Entity<Slide>()
            // .HasDiscriminator<string>("slide_type")
            // .HasValue<Blog>("blog_base")
            // .HasValue<RssBlog>("blog_rss");
        // }

    }

    public class Staff {

        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string pwd { get; set; }
        public string image_src { get; set; }
        public Boolean is_deleted { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime modified_date { get; set; }
        public string modified_by { get; set; }

    }
    public class Developer {

        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string name { get; set; }
        public string image_src { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string website { get; set; }
        // public string pic_name { get; set; }
        // public string pic_phone { get; set; }
        public Boolean is_deleted { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime modified_date { get; set; }
        public string modified_by { get; set; }

        // public List<Project> projects { get; set; }
    }

    public class Project {

        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string project_name { get; set; }
        public string project_content { get; set; } //Description,Accessbility,Siteplan, Facility
        public string location { get; set; }
        public int available_unit { get; set; }
        public string price { get; set; }
        public string image_src { get; set; }
        public Boolean is_new_project { get; set; }
        public Boolean is_publish { get; set; }
        public DateTime release_date { get; set; }

        public Boolean is_deleted { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime modified_date { get; set; }
        public string modified_by { get; set; }

        public int developer_id { get; set; }

        [ForeignKey ("developer_id")]
        public Developer developer { get; set; }

        public List<Unit> units { get; set; }
        public List<Slideshow> slideshow { get; set; }

    }

// public class ProjectSlideshow: Slideshow  { 

// }
    public class Slideshow {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int idx { get; set; }
        public string caption { get; set; }
        public string image_src { get; set; }
        public bool is_video { get; set; }
        public Boolean is_deleted { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime modified_date { get; set; }
        public string modified_by { get; set; }

    }
    public class ImageAttr {
        public string filename { get; set; }
        public string extension { get; set; }
        public byte[] byteArr { get; set; }

    }

    public class MultiPart {
        public ImageAttr imageAttr { get; set; }
    }

    public class MultiPartDeveloper : MultiPart {
        public Developer developer { get; set; }
    }

    public class MultiPartProject : MultiPart {
        public Project project { get; set; }
        public List<MultiPartSlideshow> MultiPartSlideshow { get; set; }
        public List<MultiPartUnit> MultiPartUnits { get; set; }

    }

    public class Unit {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int idx { get; set; }
        public string unit_name { get; set; }
        public string price { get; set; }
        public string image_src { get; set; }
        public int surface_area { get; set; }
        public int building_area { get; set; }
        public int bed_room { get; set; }
        public int bath_room { get; set; }
        public int car_port { get; set; }
        public string spec_content { get; set; }
        public Boolean is_deleted { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime modified_date { get; set; }
        public string modified_by { get; set; }

        public int project_id { get; set; }

        [ForeignKey ("project_id")]
        public Project project { get; set; }
        public ICollection<Slideshow> slideshow { get; set; }
        public ICollection<GaleryPhoto> galery_photos { get; set; }
        public ICollection<GaleryVideo> galery_videos { get; set; }
        public ICollection<GaleryFloorPlan> galery_floor_plan { get; set; }
        public ICollection<GaleryProgress> galery_progress { get; set; }
    }

    public class MultiPartUnit : MultiPart {
        public Unit unit { get; set; }
        public List<MultiPartGaleryPhoto> MultiPartGaleryPhoto { get; set; }
        public List<MultiPartGaleryFloorPlan> MultiPartGaleryFloorPlan { get; set; }
        public List<MultiPartGaleryVideo> MultiPartGaleryVideo { get; set; }
        public List<MultiPartGaleryProgres> MultiPartGaleryProgress { get; set; }

    }

    public class UnitPost_x {
        public string unit_name { get; set; }
        public string price { get; set; }
        public string image_src { get; set; }
        public int surface_area { get; set; }
        public int building_area { get; set; }
        public int bed_room { get; set; }
        public int car_port { get; set; }
        public string spec_content { get; set; }
        public string _image { get; set; }

    }

    public class Slide {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int idx { get; set; }
        public string caption { get; set; }
        public string image_src { get; set; }
        // public string slide_type { get; set; }
        public bool is_video { get; set; }

        public Boolean is_deleted { get; set; }
        public DateTime created_date { get; set; }
        public string created_by { get; set; }
        public DateTime modified_date { get; set; }
        public string modified_by { get; set; }

        public int unit_id { get; set; }

        [ForeignKey ("unit_id")]
        public Unit unit { get; set; }
    }

    // public class GaleryPhoto : Slide { }
    public class MultiPartSlideshow : MultiPart {
        public Slide slide { get; set; }
    }

    public class GaleryPhoto : Slide { }
    public class MultiPartGaleryPhoto : MultiPart {
        public GaleryPhoto galeryPhoto { get; set; }
    }

    public class GaleryFloorPlan : Slide { }
    public class MultiPartGaleryFloorPlan  : MultiPart {
        public GaleryFloorPlan galeryFloorPlan { get; set; }
    }

    public class GaleryVideo : Slide { }
    public class MultiPartGaleryVideo : MultiPart {
        public GaleryVideo galeryVideo { get; set; }
    }

    public class GaleryProgress : Slide { }
    public class MultiPartGaleryProgres : MultiPart {
        public GaleryProgress galeryProgress { get; set; }
    }

}