using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LivrETS.Models
{
    public class FairStep
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public FairPhase CodeStep;
        [Required]
        public string Place { get; set; }
        [Required]
        public DateTime StartDateTime { get; set; }
        [Required]
        public DateTime EndDateTime { get; set; }

        [NotMapped]
        public string TypeName
        {
            get
            {
                switch (CodeStep)
                {
                    case FairPhase.PICKING:
                        return "Ceuillette";
                    case FairPhase.SALE:
                        return "Vente";
                    case FairPhase.RETRIEVAL:
                        return "Récupération";
                    default:
                        return string.Empty;
                }
            }
        }
    }
}