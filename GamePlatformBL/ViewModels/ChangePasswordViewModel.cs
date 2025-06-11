using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePlatformBL.ViewModels
{
    public class ChangePasswordViewModel
    {
        public int Id { get; set; }
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
