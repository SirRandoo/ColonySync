// MIT License
//
// Copyright (c) 2025 sirrandoo
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ColonySync.Common.Extensions;

/// <summary>
///     Provides extension methods for working with <see cref="Result" /> and <see cref="Result{T}" /> types, enabling
///     convenient transformations, async handling, and actions on success or failure.
/// </summary>
[PublicAPI]
public static class ResultExtensions
{
    /// <summary>Converts the given value to a successful <see cref="Result{T}" /> instance.</summary>
    /// <typeparam name="T">The type of the value to be encapsulated in the result.</typeparam>
    /// <param name="value">The value to be wrapped in a successful result.</param>
    /// <returns>A <see cref="Result{T}" /> representing a successful operation containing the provided value.</returns>
    public static Result<T> ToResult<T>(this T value)
    {
        return Result.Ok(value);
    }

    /// <summary>
    ///     Converts a task of type <typeparamref name="T" /> to a <see cref="Result{T}" /> instance, handling exceptions
    ///     and wrapping the result in a success or failure outcome.
    /// </summary>
    /// <typeparam name="T">The type of the result produced by the task.</typeparam>
    /// <param name="task">The task to be converted to a <see cref="Result{T}" />.</param>
    /// <returns>
    ///     A <see cref="Result{T}" /> representing the operation's success with the result value or a failure with the
    ///     exception information if the task fails.
    /// </returns>
    public static async Task<Result<T>> ToResultAsync<T>(this Task<T> task)
    {
        try
        {
            var result = await task;

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            return Result.Fail<T>(ex);
        }
    }

    /// <summary>Executes the specified action if the result represents a successful operation.</summary>
    /// <typeparam name="T">The type of the value contained within the result.</typeparam>
    /// <param name="result">The result to evaluate.</param>
    /// <param name="action">The action to execute if the operation was successful.</param>
    /// <returns>The original result, regardless of success or failure.</returns>
    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        if (result.Success) action(result.Value!);

        return result;
    }

    /// <summary>Executes the specified action if the result indicates a failure.</summary>
    /// <typeparam name="T">The type of the value encapsulated in the result.</typeparam>
    /// <param name="result">The result to evaluate for failure.</param>
    /// <param name="action">
    ///     The action to execute if the result is a failure. The action receives the error message as a
    ///     parameter.
    /// </param>
    /// <returns>The original <see cref="Result{T}" /> instance, to enable method chaining.</returns>
    public static Result<T> OnFailure<T>(this Result<T> result, Action<string?> action)
    {
        if (result.Failure) action(result.Error);

        return result;
    }
}