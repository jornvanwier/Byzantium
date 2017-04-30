using Assets.Scripts.Game.Units.Groups;
using UnityEngine;

namespace Assets.Scripts.Game.Units.Controllers
{
    public class InputController : UnitController
    {
        public override bool IsAi { get; } = false;

        protected override void ControllerAttack()
        {
            // Don't do anything, formation can only be changed manually
        }

        protected override void ConsiderFormation(Contubernium unit)
        {
            // Don't do anything, formation can only be changed manually
        }

        protected override void ControllerTick()
        {
            if (!IsSelected || !Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKey(KeyCode.LeftAlt)) return;

            var plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.ScreenPointToRay(Input.mousePosition);

            if (!plane.Raycast(ray, out float rayDistance)) return;

            Vector3 intersection = ray.GetPoint(rayDistance);

            if (Input.GetKey(KeyCode.LeftControl))
                Teleport(intersection);
            else
                Goal = MapRenderer.WorldToCubicalCoordinate(intersection);
        }
    }
}