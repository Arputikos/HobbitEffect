using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public abstract class PlayerControllerUtilities
    {
        protected PlayerController _pc;
        protected Transform _transform;

        public virtual void Initialize(PlayerController playerController, Transform transform)
        {
            _pc = playerController;
            _transform = transform;
        }
    }
}
