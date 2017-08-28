using System;
using System.Collections.Generic;
using System.Linq;
using CollisionDetectionStuff.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CollisionDetectionStuff
{
    public class UserInputHandler
    {
        private const float Velocity = 4f;

        public void Handle(Entity entity, List<Entity> entities, KeyboardState keyboardState)
        {
            float vX = 0;
            float vY = 0;
            if (keyboardState.IsKeyDown(Keys.W))
                vY = -Velocity;
            if (keyboardState.IsKeyDown(Keys.S))
                vY = +Velocity;
            if (keyboardState.IsKeyDown(Keys.A))
                vX = -Velocity;
            if (keyboardState.IsKeyDown(Keys.D))
                vX = +Velocity;

            var newVelocity = new Vector2(vX, vY);
            
            entity.Velocity = newVelocity;

            CheckAndResolveCollisions(entities);

            entity.Position += entity.Velocity;
        }


       private static void CheckAndResolveCollisions(List<Entity> entities)
        {
            //Only check moving entities
            foreach (var entity in entities.Where(x => x.Velocity != Vector2.Zero))
                CheckCollisionWithOtherEntities(entity, entities);
        }

        private static void CheckCollisionWithOtherEntities(Entity entity, IEnumerable<Entity> allOtherEntities)
        {
            foreach (var otherEntity in allOtherEntities.Where(x => !x.Equals(entity)))
            {
                if (CheckAndResolveCollisionWithOtherEntity(entity, otherEntity))
                {
                    break;
                }
            }
        }

        private static bool CheckAndResolveCollisionWithOtherEntity(Entity entity, Entity otherEntity)
        {
            var offsetRectangle = entity.Bounds;

            // offset the current entity bounds to the furthest location possible with the current velocity
            offsetRectangle.Offset(entity.Velocity.X, entity.Velocity.Y);

            if (!offsetRectangle.Intersects(otherEntity.Bounds)) return false;

            var onlyVertical = entity.Velocity.Y != 0 && entity.Velocity.X == 0;
            var onlyHorizontal = entity.Velocity.Y == 0 && entity.Velocity.X != 0;

            if (onlyHorizontal)
            {
                return CheckHorizontalCollision(entity, otherEntity);
            }

            if (onlyVertical)
            {
                return CheckVerticalCollision(entity, otherEntity);
            }


            float biggest = 0;
            float smallest = 0;

            var movesMoreHorizontally = false;

            var velX = entity.Velocity.X;
            var absVelX = Math.Abs(velX);
            
            var velY = entity.Velocity.Y;
            var absVelY = Math.Abs(velY);

            int sign = 0;

            if (absVelX >= absVelY)
            {
                movesMoreHorizontally = true;

                sign = Math.Sign(velX);
                biggest = absVelX;
                smallest = 1/absVelY;
            }
            else if (absVelX < absVelY)
            {
                sign = Math.Sign(velY);
                biggest = absVelY;
                smallest = 1 / absVelX;
            }          

            for (var i = 0; i < biggest; i++)
            {
                var o2 = entity.Bounds;
                if (movesMoreHorizontally)
                {
                    o2.Offset(i*sign + sign, smallest * i * sign);
                }
                else
                {
                    o2.Offset(smallest * i * sign, i*sign + sign);
                }

                if (o2.Intersects(otherEntity.Bounds))
                {
                    if (movesMoreHorizontally)
                    {
                        entity.Velocity = new Vector2(i * sign, smallest * i * sign);
                        return true;
                    }
                    else
                    {
                        entity.Velocity = new Vector2(smallest * i * sign, i * sign);
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool CheckHorizontalCollision(Entity entity, Entity otherEntity)
        {
            var offset = Math.Sign(entity.Velocity.X);

            var predictedEntityRectangle = entity.Bounds;
            predictedEntityRectangle.Offset(entity.Velocity.X + offset, 0);

            if (predictedEntityRectangle.Intersects(otherEntity.Bounds))
            {
                if (ResolveHorizontalCollision(entity, otherEntity, (int) entity.Velocity.X + offset))
                    return true;
            }
            return false;
        }

        private static bool ResolveHorizontalCollision(Entity entity, Entity otherEntity, int maxHorizontalDistance)
        {
            var directionModifier = Math.Sign(maxHorizontalDistance);
            for (var d = 0; d < Math.Abs(maxHorizontalDistance); d++)
            {
                var predictedEntityRectangle = entity.Bounds;
                predictedEntityRectangle.Offset(directionModifier * d, 0);

                if (!predictedEntityRectangle.Intersects(otherEntity.Bounds)) continue;

                entity.Velocity = new Vector2(directionModifier * d - 1 * directionModifier, entity.Velocity.Y);
                return true;
            }
            return false;
        }

        private static bool CheckVerticalCollision(Entity entity, Entity otherEntity)
        {
            var offset = Math.Sign(entity.Velocity.Y);

            var predictedEntityRectangle = entity.Bounds;
            predictedEntityRectangle.Offset(0, entity.Velocity.Y + offset);

            if (predictedEntityRectangle.Intersects(otherEntity.Bounds))
            {
                ResolveVerticalCollision(entity, otherEntity, (int) entity.Velocity.Y + offset);
                if (entity.Velocity.Y > 0)
                    entity.IsGrounded = true;
                return true;
            }
            return false;
        }

        private static void ResolveVerticalCollision(Entity entity, Entity otherEntity, int maxVerticalDistance)
        {
            var directionModifier = Math.Sign(maxVerticalDistance);
            for (var d = 0; d < Math.Abs(maxVerticalDistance); d++)
            {
                var predictedEntityRectangle = entity.Bounds;
                predictedEntityRectangle.Offset(0, directionModifier * d);

                if (!predictedEntityRectangle.Intersects(otherEntity.Bounds)) continue;

                entity.Velocity = new Vector2(entity.Velocity.X, directionModifier * d - directionModifier);
                break;
            }
        }
    }
}