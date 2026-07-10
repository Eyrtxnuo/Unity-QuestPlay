namespace CowRun
{
    public class SpeedSetEffect : CollisionEffect
    {
        public float speed;
        private CollisionType _type  = CollisionType.SpeedSet;

        public SpeedSetEffect(float speed)
        {
            this.speed = speed;
        }

        CollisionType CollisionEffect.type
        {
            get => _type;
            set => _type = value;
        }
    }
}