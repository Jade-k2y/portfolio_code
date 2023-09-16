using System;


namespace Studio.Game
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class PopupAttribute : Attribute
    {
        public string label { get; private set; }
        public string category { get; private set; }
        public bool keepAlive { get; private set; }
        public bool destroyOnDisabled { get; private set; }


        public PopupAttribute(string label, string category, bool keepAlive = false, bool destroyOnDisabled = false)
        {
            this.label = label;
            this.category = category;
            this.keepAlive = keepAlive || destroyOnDisabled;
            this.destroyOnDisabled = destroyOnDisabled;
        }
    }
}