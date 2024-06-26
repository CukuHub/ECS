using Entitas;
#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
#endif

namespace Cuku.ECS
{
    /// <summary>
    /// Structure to be used when serializing / deserializing an entity to / from json.
    /// </summary>
    public struct ContextData
    {
#if UNITY_EDITOR && ODIN_INSPECTOR
        [PropertyOrder(0), HideLabel, DisplayAsString]
#endif
        /// <summary>
        /// Entity's context name, to be matched to the actual <see cref="IContext"/>.
        /// </summary>
        public string Context;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [PropertyOrder(1)]
        [Button(ButtonSizes.Large, ButtonStyle.Box, Expanded = true)]
        private void AddEntity()
        {
            Array.Resize(ref Entities, Entities.Length + 1);
            ValidateEntities();
        }

        [PropertyOrder(2), PropertySpace(SpaceAfter = 20), ListDrawerSettings(HideAddButton = true)]
        [InfoBox("Only one Component Type per Entity is allowed!", InfoMessageType.Error,
            nameof(duplicateComponents))]
        [OnValueChanged(nameof(ValidateEntities), true)]
        [ValueDropdown(nameof(Components))]
#endif
        /// <summary>
        /// Entities are collection of Components.
        /// </summary>
        public IComponent[][] Entities;

#if UNITY_EDITOR && ODIN_INSPECTOR
        [UnityEngine.HideInInspector]
        private bool duplicateComponents;

        public bool IsValid() => !duplicateComponents;

        private IEnumerable<IComponent> Components()
        {
            var components = new List<IComponent>();
            foreach (var componentType in ComponentTypes())
            {
                components.Add((IComponent)Activator.CreateInstance(componentType));
            }
            return components;
        }

        private void ValidateEntities()
        {
            duplicateComponents = false;
            var componentTypes = ComponentTypes();

            for (int i = 0; i < Entities.Length; i++)
            {
                // Initialize null or empty entities
                if (Entities[i] == null || Entities[i].Length < 1)
                {
                    Entities[i] = new IComponent[1];
                }

                // Initialize null Components
                for (int j = 0; j < Entities[i].Length; j++)
                {
                    if (Entities[i][j] == null)
                    {
                        Entities[i][j] = (IComponent)Activator.CreateInstance(componentTypes[0]);
                    }
                }
            }

            foreach (var entity in Entities)
            {
                for (int i = 0; i < entity.Length; i++)
                {
                    // Verify that there are no duplicate Components
                    var componentTypeCount = 0;
                    foreach (var otherComponent in entity)
                    {
                        if (otherComponent.GetType() == entity[i].GetType())
                        {
                            componentTypeCount++;
                        }
                        if (componentTypeCount > 1)
                        {
                            duplicateComponents = true;
                            return;
                        }
                    }
                }
            }
        }

        private Type[] ComponentTypes()
            => ((IContext)this.ContextType().Instance()).ContextInfo.ComponentTypes;
#endif
    }
}
