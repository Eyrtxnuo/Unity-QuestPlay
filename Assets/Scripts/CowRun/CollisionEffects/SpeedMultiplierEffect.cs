namespace CowRun
{
    public class SpeedMultiplierEffect : CollisionEffect
    {
        public float speedMultiplier;
        private CollisionType _type  = CollisionType.SpeedMultiplier;

        public SpeedMultiplierEffect(float speedMultiplier)
        {
            this.speedMultiplier = speedMultiplier;
        }

        CollisionType CollisionEffect.type
        {
            get => _type;
            set => _type = value;
        }
    }
}