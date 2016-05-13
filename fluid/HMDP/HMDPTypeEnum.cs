namespace fluid.HMDP
{
    public enum HMDPTypeEnum
    {
        icon, heat, throughtput, solid, merged, dust
    }

    public class HMDPTypeHelper
    {
        public static bool isRotateable(HMDPTypeEnum en)
        {
            return !(en == HMDPTypeEnum.heat || en == HMDPTypeEnum.icon);
        }
    }
}
