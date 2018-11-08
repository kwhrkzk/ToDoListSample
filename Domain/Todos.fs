namespace Domain

open System
open Reactive.Bindings.Notifiers
open Reactive.Bindings

type TaskID =
    {
        id: Guid
    }
    static member NewID () = { id = Guid.NewGuid() }
    override this.ToString () = this.id.ToString()

type Title =
    {
        title: string
    }
    override this.ToString () = this.title
    static member Create (_title:string) = { title = _title }

type Explain =
    {
        explain: string
    }
    override this.ToString () = this.explain
    static member Create (_explain:string) = { explain = _explain }
    static member Empty = { explain = "" }

type StatusEnum = 
    | Ready = 1
    | Doing = 2
    | Done = 3

type Status =
    {
        status: StatusEnum
    }
    static member Ready = { status = StatusEnum.Ready }
    static member Doing = { status = StatusEnum.Doing }
    static member Done = { status = StatusEnum.Done }

type Task =
    {
        ID: TaskID
        Title: Title
        Explain: Explain
        Status: Status
    }
    member this.TitleString = this.Title.ToString()
    member this.ExplainString = this.Explain.ToString()

type Tasks =
    inherit System.Collections.Generic.List<Task>
    new () =
        {
            inherit System.Collections.Generic.List<Task>()
        }
    new (list:seq<Task>) =
        {
            inherit System.Collections.Generic.List<Task>(list)
        }

type ITaskRepository =
    abstract All: unit -> Tasks
    abstract Unshift: Task -> unit
    abstract Get: TaskID -> Task
    abstract Replace: Task -> unit
    abstract Replace: TaskID * Status -> Task

type ITaskFactory =
    abstract Create: string -> Task
    abstract Replace: Explain * Task -> Task
    abstract Replace: Title * Task -> Task
    abstract Replace: Status * Task -> Task

type Message () = class end

type DomainEvent () =
    inherit Message()
    member this.TimeStamp: DateTime = DateTime.Now

type AddedTaskEvent (_task:Task) =
    inherit DomainEvent()

    member this.Task = _task

type AddTaskCommandParam (_task:Task) =
    member this.Task = _task

type AddTaskCommand (_param:AddTaskCommandParam, _taskRepository:ITaskRepository, _messageBroker:IMessageBroker) =
    inherit ReactiveCommand<AddTaskCommandParam>()

    member this.Param = _param

    member this.Execute () =
        _taskRepository.Unshift(this.Param.Task)
        _messageBroker.Publish<AddedTaskEvent>(AddedTaskEvent(this.Param.Task))
        base.Execute(this.Param)

type EditedTaskEvent (_task:Task) =
    inherit DomainEvent()

    member this.Task = _task

type EditTaskCommandParam (_task:Task) =
    member this.Task = _task

type EditTaskCommand (_param:EditTaskCommandParam, _taskRepository:ITaskRepository, _messageBroker:IMessageBroker) =
    inherit ReactiveCommand<EditTaskCommandParam>()

    member this.Param = _param

    member this.Execute () =
        _taskRepository.Replace(this.Param.Task)
        _messageBroker.Publish<EditedTaskEvent>(EditedTaskEvent(this.Param.Task))
        base.Execute(this.Param)

type ChangedTaskEvent (_task:Task) =
    inherit DomainEvent()

    member this.Task = _task

type ChangeStatusCommandParam (_taskid:TaskID, _status:Status) =
    member this.TaskID = _taskid
    member this.Status = _status

type ChangeStatusCommand (_param:ChangeStatusCommandParam, _taskRepository:ITaskRepository, _messageBroker:IMessageBroker) =
    inherit ReactiveCommand<ChangeStatusCommandParam>()

    member this.Param = _param

    member this.Execute () =
        let task = _taskRepository.Replace(this.Param.TaskID, this.Param.Status)
        _messageBroker.Publish<ChangedTaskEvent>(ChangedTaskEvent(task))
        base.Execute(this.Param)

type ITaskCommandFactory =
    abstract AddTaskCommand: AddTaskCommandParam -> AddTaskCommand
    abstract EditTaskCommand: EditTaskCommandParam -> EditTaskCommand
    abstract ChangeStatusCommand: ChangeStatusCommandParam -> ChangeStatusCommand
