namespace EmailSaas.Application.Common.Models;

public class Result
{
    public bool Succeeded { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();

    public Result() { }

    protected Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    public static Result Success() => new(true, Array.Empty<string>());
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
    public static Result Failure(string error) => new(false, new[] { error });
}

public class Result<T> : Result
{
    public T? Data { get; set; }

    public Result() { }

    protected Result(bool succeeded, T? data, IEnumerable<string> errors)
        : base(succeeded, errors)
    {
        Data = data;
    }

    public static Result<T> Success(T data) => new(true, data, Array.Empty<string>());
    public new static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);
    public new static Result<T> Failure(string error) => new(false, default, new[] { error });
}