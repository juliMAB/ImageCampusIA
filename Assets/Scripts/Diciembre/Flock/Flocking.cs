using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using TMPro;

namespace Diciembre
{
    public class Flocking : MonoBehaviour
    {
        #region EXPOSED_FIELDS

        public float maxSpeed;
        public float maxForce;
        public float checkRadious;

        public float separationMultiplayer;
        public float cohesionMultiplayer;
        public float aligmentMultiplayer;

        public Vector2 velocity;
        public Vector2 aceleration;
        #endregion

        #region PRIVATE_FIELDS
        private bool onDestination=false;
        public Vector3 target;
        #endregion

        #region PROPERPTIES
        private Vector2 Position
        {
            get
            {
                return gameObject.transform.position;
            }
            set
            {
                gameObject.transform.position = value;
            }
        }
        #endregion

        #region UNITY_CALLS

        private void Update()
        {
            if (Vector3.Distance(Position, target) < 0.05f)
                return;
            Collider2D[] otherColliders = Physics2D.OverlapCircleAll(Position, checkRadious);
            List<Flocking> boids = otherColliders.Select(others => others.GetComponent<Flocking>()).ToList();
            boids.Remove(this);

            if (boids.Any())
            {                
                aceleration = Alignment(boids) * aligmentMultiplayer + Separation(boids) * separationMultiplayer + Cohesion(boids) * cohesionMultiplayer + GoDestination();
            }
            else
            {
                aceleration = GoDestination();
            }

            velocity += aceleration;
            velocity = LimitMagnitude(velocity, maxSpeed);
            Position += velocity * Time.deltaTime;
        }
        #endregion

        #region PUBLIC_METHODS
        public void SetTarget(Vector3 target)
        {
            this.target = target;
        }
        public void StopMoving()
        {
            velocity = Vector2.zero;
            aceleration = velocity;
        }
        #endregion

        #region PRIVATE_METHODS
        private Vector2 Alignment(IEnumerable<Flocking> boids)
        {
            Vector2 velocity = Vector2.zero;

            foreach (Flocking boid in boids)
            {
                velocity += boid.velocity;
            }
            velocity /= boids.Count();

            return Steer(velocity.normalized * maxSpeed);
        }

        private Vector2 Cohesion(IEnumerable<Flocking> boids)
        {
            Vector2 sumPositions = Vector2.zero;
            foreach (Flocking boid in boids)
            {
                sumPositions += boid.Position;
            }
            Vector2 average = sumPositions / boids.Count();
            Vector2 direction = average - Position;

            return Steer(direction.normalized * maxSpeed);
        }

        private Vector2 Separation(IEnumerable<Flocking> boids)
        {
            Vector2 direction = Vector2.zero;
            boids = boids.Where(o => DistanceTo(o) <= checkRadious / 2);

            foreach (var boid in boids)
            {
                Vector2 difference = Position - boid.Position;
                direction += difference.normalized / difference.magnitude;
            }
            direction /= boids.Count();

            return Steer(direction.normalized * maxSpeed);
        }

        private Vector2 Steer(Vector2 desired)
        {
            Vector2 steer = desired - velocity;
            return LimitMagnitude(steer, maxForce);
        }

        private float DistanceTo(Flocking boid)
        {
            return Vector3.Distance(boid.transform.position, Position);
        }

        private Vector2 LimitMagnitude(Vector2 baseVector, float maxMagnitude)
        {
            if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
            {
                baseVector = baseVector.normalized * maxMagnitude;
            }
            return baseVector;
        }

        private Vector2 GoDestination()
        {
            if (target != Vector3.one * -1)
                return ((Vector2)target - Position).normalized;
            return Vector2.zero;
        }
        private void TpDestination()
        {
           
        }
        #endregion
    }
}