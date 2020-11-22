using System;

namespace EEMod.MachineLearning
{
    [Serializable]
    public class MLObject
    {
        public void Initialize()
        {
            OnInitialize();
        }

        public void Update()
        {
            OnUpdate();
        }

        public virtual void OnInitialize()
        {
            ;
        }

        public virtual void OnUpdate()
        {
            ;
        }

        public MLObject()
        {
            Initialize();
        }
    }
}