#region

using System;
using UnityEngine;

#endregion

namespace Crysc.Presentation
{
    public class ParallaxRegistrar : MonoBehaviour
    {
        [field: SerializeField] public ParallaxLayerConfig Layer { get; set; }
        [field: SerializeField] public bool IsAffectedBySpeed { get; set; }

        [NonSerialized] private bool _registered;

        private void Start()
        {
            Register(layer: Layer, isAffectedBySpeed: IsAffectedBySpeed);
        }

        private void OnDestroy()
        {
            Deregister();
        }

        public void Register(ParallaxLayerConfig layer, bool isAffectedBySpeed = true)
        {
            if (!ParallaxSystem.I)
                return;

            if (_registered)
            {
                if ((Layer == layer) && (IsAffectedBySpeed == isAffectedBySpeed))
                    return;
                Deregister();
            }

            Layer = layer;
            IsAffectedBySpeed = isAffectedBySpeed;
            if (Layer == null)
                return;

            ParallaxSystem.I.Register(
                layer: layer,
                registrant: transform,
                isAffectedBySpeed: isAffectedBySpeed
            );

            _registered = true;
        }

        public void Deregister()
        {
            if (!_registered || !ParallaxSystem.I)
                return;

            ParallaxSystem.I.Deregister(
                layer: Layer,
                registrant: transform
            );

            _registered = false;
        }
    }
}
