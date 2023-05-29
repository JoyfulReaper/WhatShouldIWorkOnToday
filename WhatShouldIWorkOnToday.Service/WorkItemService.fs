module WorkItemService
    open Giraffe

    let getWorkItemHandler : HttpHandler =
        text "Hello World"

    let sayHelloWorld (name : string) : HttpHandler =
        let greeting = sprintf "Hello World, from %s" name
        text greeting