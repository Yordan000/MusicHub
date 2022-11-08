using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MusicHub.Data.Common;

namespace MusicHub.Data.Models
{
    public class Performer
    {
        public Performer()
        {
            this.PerformerSongs = new HashSet<SongPerformer>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(GlobalConstrains.PerformerNameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(GlobalConstrains.PerformerNameMaxLength)]
        public string LastName { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public decimal NetWorth { get; set; }

        public virtual ICollection<SongPerformer> PerformerSongs { get; set; }
    }
}
