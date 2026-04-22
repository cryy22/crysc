using System;
using UnityEngine;

namespace Crysc.Presentation
{
    public class ParallaxRegistrar : MonoBehaviour
    {
        [field: SerializeField] public ParallaxLayerConfig Layer { get; set; }
        [field: SerializeField] public bool IsAffectedBySpeed { get; set; }
        
        [NonSerialized] private ParallaxLayerConfig _registeredLayer;

        private void Start()
        {
            if (!_registeredLayer)
                Register(Layer, isAffectedBySpeed: IsAffectedBySpeed);
        }

        private void OnDestroy()
        {
            Deregister();
        }

        public void Register(ParallaxLayerConfig layer, bool isAffectedBySpeed = true)
        {
            if (_registeredLayer == layer)
                return;
            if (_registeredLayer)
                Deregister();

            if (ParallaxSystem.I)
            {
                ParallaxSystem.I.Register(
                    layer: layer,
                    registrant: transform,
                    isAffectedBySpeed: isAffectedBySpeed
                );
                
                _registeredLayer = layer;
            }
        }

        public void Deregister()
        {
            if (ParallaxSystem.I && _registeredLayer)
            {
                ParallaxSystem.I.Deregister(
                    layer: _registeredLayer,
                    registrant: transform
                );
                
                _registeredLayer = null;
            }
        }
    }
}
