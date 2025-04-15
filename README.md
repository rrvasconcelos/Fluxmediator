# FluxMediator

FluxMediator é uma alternativa ao MediatR para implementar o padrão Mediator em aplicações .NET. Ele foi projetado para simplificar a comunicação entre componentes, promovendo um design mais limpo e desacoplado.

## Recursos

- Implementação leve e fácil de usar.
- Suporte para comandos, consultas e notificações.
- Promove a separação de responsabilidades e facilita testes unitários.

## Instalação

Adicione o pacote NuGet ao seu projeto:

```bash
dotnet add package FluxMediator
```

## Uso Básico

### Configuração

Registre o FluxMediator no seu contêiner de injeção de dependência:

```csharp
// Registra todos os assemblies no AppDomain
services.AddMyMediator();

// Registra assemblies específicos
services.AddMyMediator(typeof(Program).Assembly, typeof(Startup).Assembly);

// Registra por prefixo de namespace
services.AddMyMediator("MyCompany.MyProject");

// Registra por tipos marcadores
services.AddMyMediator(typeof(Program), typeof(SomeHandler));
```

### Exemplo de Comando

Defina um comando e seu respectivo manipulador:

```csharp
public class MyCommand : ICommand
{
    public string Data { get; set; }
}

public class MyCommandHandler : ICommandHandler<MyCommand>
{
    public Task Handle(MyCommand command, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Comando recebido: {command.Data}");
        return Task.CompletedTask;
    }
}
```

Envie o comando:

```csharp
await mediator.Send(new MyCommand { Data = "Olá, FluxMediator!" });
```

## Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues ou enviar pull requests.

## Licença

Este projeto está licenciado sob a [MIT License](LICENSE).