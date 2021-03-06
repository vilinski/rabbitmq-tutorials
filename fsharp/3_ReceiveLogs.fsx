#r "packages/RabbitMQ.Client/lib/net40/RabbitMQ.Client.dll"

open System
open RabbitMQ.Client
open RabbitMQ.Client.Events
open System.Text

let factory = new ConnectionFactory(HostName = "localhost")
(
    use connection = factory.CreateConnection()
    use channel = connection.CreateModel()

    channel.ExchangeDeclare("logs", "fanout")
    let queueName = channel.QueueDeclare().QueueName
    channel.QueueBind(queue = queueName,
                      exchange = "logs",
                      routingKey = " ")

    printfn " [*] Waiting for logs."

    let receive (ea:BasicDeliverEventArgs) =
        let message = Encoding.UTF8.GetString ea.Body
        printfn " [x] %s" message

    let consumer =  EventingBasicConsumer channel
    consumer.Received.Add(receive)

    channel.BasicConsume(queue = "task_queue",
                         noAck = false,
                         consumer = consumer) |> ignore

    printfn " Press [enter] to exit"
    Console.ReadLine() |> ignore
)
