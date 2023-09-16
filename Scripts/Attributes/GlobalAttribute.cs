using System;


namespace Studio.Game
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class GlobalAttribute : Attribute
    {
        public bool generate { get; private set; }
        public bool permanent { get; private set; }


        public GlobalAttribute(bool generate = true, bool permanent = false)
        {
            this.generate = generate;
            this.permanent = permanent;
        }
    }
}