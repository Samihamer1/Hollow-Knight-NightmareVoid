using System.Reflection;

namespace NightmareVoid
{
    public static class Helper
    {
        
        public static FsmFloat CreateFsmFloat(this PlayMakerFSM fsm, string floatName, float value)
        {
            var @new = new FsmFloat(floatName);
            @new.Value = value;

            fsm.FsmVariables.FloatVariables = fsm.FsmVariables.FloatVariables.Append(@new).ToArray();

            return @new;
        }

        public static FsmEvent CreateFsmEvent(this PlayMakerFSM fsm, string eventName)
        {
            var @new = new FsmEvent(eventName);

            fsm.Fsm.Events = fsm.Fsm.Events.Append(@new).ToArray();

            return @new;
        }

        public static FsmVector3 CreateFsmVector3(this PlayMakerFSM fsm, string Name)
        {
            var @new = new FsmVector3(Name);

            fsm.Fsm.Variables.Vector3Variables = fsm.Fsm.Variables.Vector3Variables.Append(@new).ToArray();

            return @new;
        }

        public static FsmEvent CreateFsmEvent(this PlayMakerFSM fsm, FsmEvent fsmEvent)
        {

            fsm.Fsm.Events = fsm.Fsm.Events.Append(fsmEvent).ToArray();

            return fsmEvent;
        }

        public static void AddGlobalTransition(this PlayMakerFSM fsm, FsmEvent @event, string toState)
        {
            fsm.Fsm.GlobalTransitions = fsm.Fsm.GlobalTransitions.Append(new FsmTransition
            {
                FsmEvent = @event,
                ToFsmState = fsm.Fsm.GetState(toState)
            }).ToArray();
        }

        public static FsmEvent GetFsmEvent(this PlayMakerFSM fsm, string eventName)
        {
            foreach (FsmEvent Event in fsm.Fsm.Events)
            {
                if (Event.Name == eventName) { return Event; }

            }

            return null;
        }

        public static FsmGameObject CreateFsmGameObject(this PlayMakerFSM fsm, string objname)
        {
            var @new = new FsmGameObject(objname);

            FsmGameObject[] newlist = new FsmGameObject[fsm.FsmVariables.GameObjectVariables.Length + 1];

            int counter = 0;
            foreach (FsmGameObject item in fsm.FsmVariables.GameObjectVariables)
            {
                newlist[counter] = item;
                counter += 1;
            }
            newlist[counter] = @new;

            fsm.FsmVariables.GameObjectVariables = newlist;

            return @new;
        }
    }
}
