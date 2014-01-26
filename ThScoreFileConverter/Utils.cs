﻿//-----------------------------------------------------------------------
// <copyright file="Utils.cs" company="None">
//     (c) 2013-2014 IIHOSHI Yoshinori
// </copyright>
//-----------------------------------------------------------------------

namespace ThScoreFileConverter
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;

    /// <summary>
    /// Provides static methods for convenience.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Defines a method to read from a binary stream.
        /// </summary>
        public interface IBinaryReadable
        {
            /// <summary>
            /// Reads from a stream by using the specified <see cref="BinaryReader"/> instance.
            /// </summary>
            /// <param name="reader">The instance to use.</param>
            void ReadFrom(BinaryReader reader);
        }

        /// <summary>
        /// Concatenates all names of the specified enumeration type.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type.</typeparam>
        /// <param name="separator">The string to use as a separator.</param>
        /// <returns>
        /// A string that consists of the names of <typeparamref name="TEnum"/> delimited by
        /// <paramref name="separator"/>.
        /// </returns>
        public static string JoinEnumNames<TEnum>(string separator)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return string.Join(separator, Enum.GetNames(typeof(TEnum)));
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated
        /// constants to an equivalent enumerated instance.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type.</typeparam>
        /// <param name="value">A string containing the name or value to convert.</param>
        /// <param name="ignoreCase"><c>true</c> if ignore case; <c>false</c> to regard case.</param>
        /// <returns>
        /// An instance of <typeparamref name="TEnum"/> whose value is represented by
        /// <paramref name="value"/>.
        /// </returns>
        public static TEnum ParseEnum<TEnum>(string value, bool ignoreCase = false)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
        }

        /// <summary>
        /// Gets the <c>IEnumerable{T}</c> instance to enumerate values of the <typeparamref name="TEnum"/>
        /// type.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type.</typeparam>
        /// <returns>
        /// The <c>IEnumerable{T}</c> instance to enumerate values of the <typeparamref name="TEnum"/> type.
        /// </returns>
        public static IEnumerable<TEnum> GetEnumerator<TEnum>()
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }

        /// <summary>
        /// Returns a string that represents the specified numeric value.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="number"/>.</typeparam>
        /// <param name="number">A numeric value.</param>
        /// <param name="outputSeparator">
        /// <c>true</c> if use a thousand separator character; otherwise, <c>false</c>.
        /// </param>
        /// <returns>A string that represents <paramref name="number"/>.</returns>
        public static string ToNumberString<T>(T number, bool outputSeparator) where T : struct
        {
            return outputSeparator ? string.Format("{0:N0}", number) : number.ToString();
        }

        /// <summary>
        /// Returns a <see cref="MatchEvaluator"/> delegate that does the same evaluation with
        /// <paramref name="evaluator"/> and throws no exception.
        /// If an exception occurred from <paramref name="evaluator"/>, the returned delegate returns
        /// the matched substring itself.
        /// </summary>
        /// <param name="evaluator">The <see cref="MatchEvaluator"/> delegate to convert.</param>
        /// <returns>A converted <see cref="MatchEvaluator"/> delegate.</returns>
        public static MatchEvaluator ToNothrowEvaluator(MatchEvaluator evaluator)
        {
            return match =>
            {
                try
                {
                    return evaluator(match);
                }
                catch
                {
                    return match.ToString();
                }
            };
        }

        /// <summary>
        /// Represents a logical-and predicate.
        /// </summary>
        /// <typeparam name="T">Type of the instance to evaluate.</typeparam>
        public class And<T>
        {
            /// <summary>
            /// The predicates combined with logical-and operators.
            /// </summary>
            private Func<T, bool>[] predicates;

            /// <summary>
            /// Initializes a new instance of the <see cref="And{T}"/> class.
            /// </summary>
            /// <param name="predicates">The predicates combined with logical-and operators.</param>
            public And(params Func<T, bool>[] predicates)
            {
                this.predicates = predicates;
            }

            /// <summary>
            /// Casts the instance of the <see cref="And{T}"/> class to an instance of the
            /// <c>Func{T,bool}</c> class.
            /// </summary>
            /// <param name="from">The instance of the <see cref="And{T}"/> class to cast.</param>
            /// <returns>An instance of the <c>Func{T,bool}</c> class.</returns>
            public static implicit operator Func<T, bool>(And<T> from)
            {
                return from.Pred;
            }

            /// <summary>
            /// Evaluates the instance of <typeparamref name="T"/> by all predicates of the current instance.
            /// </summary>
            /// <param name="obj">The instance to evaluate.</param>
            /// <returns>
            /// <c>true</c> if <paramref name="obj"/> satisfies all predicates of the current instance;
            /// otherwise, <c>false</c>.
            /// </returns>
            private bool Pred(T obj)
            {
                return this.predicates.All(pred => pred(obj));
            }
        }
    }
}
