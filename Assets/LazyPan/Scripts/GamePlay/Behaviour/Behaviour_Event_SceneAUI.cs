namespace LazyPan {
    public class Behaviour_Event_SceneAUI : Behaviour {
        private Flow_SceneA flow;
        public Behaviour_Event_SceneAUI (Entity entity, string behaviourSign) : base(entity, behaviourSign) {
            Flo.Instance.GetFlow(out flow);
            flow.Login();
        }

        public override void DelayedExecute() {
            
        }

        public override void Clear() {
            base.Clear();
        }
    }
}