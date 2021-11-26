using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

#nullable disable

namespace RSS.Models
{

    [Serializable]
    [XmlRoot("Categories")]
    public partial class Categories
    {
        [XmlElement("Category")]

        public List<Category> C_Items { get; set; }
    }


  //  [Serializable]

    public partial class Category
    {
        public Category()
        {
            Feeds = new HashSet<Feed>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [XmlElement("Title")]
        public string Title { get; set; }
        [XmlIgnore]
        public virtual ICollection<Feed> Feeds { get; set; }
    }
}
