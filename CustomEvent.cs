using static System.Console;

MainClass<int> main = new MainClass<int>(123123);


public class MainClass<T>
{
    Compound<T> compound;
    public MainClass(T input)
    {
        compound =  new Compound<T>(input);
        Callback(this, compound.state);
    }

    
    public void Callback(object sender, EventArgs e)
    {
        compound.OnAddresssFunction((sender, e) => 
        { 
            WriteLine("asdasdas");
        });

        compound.OnCallBehaviour();
    }
}

/// IDisposable 구현
public class State<T> : EventArgs 
{
    public enum FlagState
    {
        None = 0,
        Validate = 1,
        InValidate = 2
    }

    T data;
    FlagState flag = FlagState.Validate;
    int validateCount = 0;

    T Data
    {
        get 
        { 
            if(data is not null)
                return data; 
            else
                throw new Exception(nameof(data));
        }
        set
        {
            data = value ?? throw new Exception(nameof(value));
        }
    }

    /// static State(T input) 을 사용하여 인스턴스 생성자보다 더 빠르게 콜
    /// 그리고 딱 한번만 호출
    public State(T input) => this.data = input;

    public static implicit operator State<T>(T value) => new State<T>(value);
    public static implicit operator T(State<T> value) => value.Data is not null ? (T)Convert.ChangeType(value.Data, typeof(T)) : throw new Exception(nameof(value));
    public class Behaviour
{
    /// 이 부분을 evnet 원형으로 설정하던지
    /// public delegate void CustomEvnetHandler(object sender, EvnetArgs e);
    /// public evnet CustomEvnetHandler handler;
    public event EventHandler<EventArgs> behaviour;

    public void AddBehaviour(Action<object, EventArgs> e)
    {
        if(e is not null)
        {
            /// 이 부분 refactoring
            /// 2번의 shallow copy
            /// 혹은 invoke
            Action<object, EventArgs> temp = e.Invoke;
            EventHandler<EventArgs> h = temp.Invoke!;
            behaviour += h;
        }
        else
        {
            throw new Exception(nameof(e));
        }
    }

    public void CallBehaviour(EventArgs e)
    {
        if(e is not null && behaviour is not null)
        {
            EventHandler<EventArgs> temp = behaviour;
            temp.Invoke(this, e);
        }
    }
}
public class Compound<T>
{
    public State<T> state { get; private set;}
    public Behaviour behaviour { get; private set;}

    public Compound(T input)
    {
        state = new State<T>(input);
        behaviour = new Behaviour();

        initialize();
    }

    void initialize()
    {

    }

    public void OnAddresssFunction(Action<object, EventArgs> e)
    {
        behaviour.AddBehaviour(e);
    }

    public void OnCallBehaviour()
    {
        behaviour.CallBehaviour(state);
    }
}
