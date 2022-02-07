using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Editor.Utilities
{
    public static class ID
    {
        //TODO : change the type from int to somthing else to support int_64, right now it is supporting only int_32
        public static int INVALID_ID => -1;
        public static bool IsValid(int id) => id != INVALID_ID;
    }
    public static class MathUtl
    {
        public static float Epsilon => 0.00001f;

        public static bool IsSame(this float? value,float? compairer)
        {
            if(!value.HasValue || !compairer.HasValue) return false;
            return System.Math.Abs(value .Value- compairer.Value) <= Epsilon;
        }
    }
}
