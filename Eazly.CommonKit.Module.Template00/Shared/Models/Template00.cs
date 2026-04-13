using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Oqtane.Models;

namespace Eazly.CommonKit.Module.Template00.Models
{
    public class Contition
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string SearchValue { get; set; }
    }

	public class ContentsConifg
	{
		public Boolean IsDisSave { get; set; } = false;
		public Boolean IsDisCreate {  get; set; } = false;
		public Boolean IsDisUpdate { get; set; } = false;
        public Boolean IsDisDelete { get; set; } = false;
		public Boolean IsDisExport { get; set; } = false;
	}

}
