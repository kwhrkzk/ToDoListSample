namespace AppService

open Domain

type ListViewDTO =
    {
        TaskID: TaskID
        Title: Title
        Status: Status
    }

type IListViewDTORepository =
    abstract All: unit -> seq<ListViewDTO>
