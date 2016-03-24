using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fluid.HMDP
{
    public enum HMDPTypeEnum
    {
        icon, heat, throughtput, solid, merged
    }

    public class HMDPTypeHelper
    {
        public static bool isRotateable(HMDPTypeEnum en)
        {
            return !(en == HMDPTypeEnum.heat || en == HMDPTypeEnum.icon);
        }
    }
}
