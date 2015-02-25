// Options.cs
// Authors:
//  Jonathan Pryor <jpryor@novell.com>
// Copyright (C) 2008 Novell (http://www.novell.com)
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Compile With:
//   gmcs -debug+ -r:System.Core Options.cs -o:NDesk.Options.dll
//   gmcs -debug+ -d:LINQ -r:System.Core Options.cs -o:NDesk.Options.dll
// The LINQ version just changes the implementation of
// OptionSet.Parse(IEnumerable<string>), and confers no semantic changes.

// A Getopt::Long-inspired option parsing library for C#.
// NDesk.Options.OptionSet is built upon a key/value table, where the
// key is a option format string and the value is a delegate that is 
// invoked when the format string is matched.
// Option format strings:
//  Regex-like BNF Grammar: 
//    name: .+
//    type: [=:]
//    sep: ( [^{}]+ | '{' .+ '}' )?
//    aliases: ( name type sep ) ( '|' name type sep )*
// Each '|'-delimited name is an alias for the associated action.  If the
// format string ends in a '=', it has a required value.  If the format
// string ends in a ':', it has an optional value.  If neither '=' or ':'
// is present, no value is supported.  `=' or `:' need only be defined on one
// alias, but if they are provided on more than one they must be consistent.
// Each alias portion may also end with a "key/value separator", which is used
// to split option values if the option accepts > 1 value.  If not specified,
// it defaults to '=' and ':'.  If specified, it can be any character except
// '{' and '}' OR the *string* between '{' and '}'.  If no separator should be
// used (i.e. the separate values should be distinct arguments), then "{}"
// should be used as the separator.
// Options are extracted either from the current option by looking for
// the option name followed by an '=' or ':', or is taken from the
// following option IFF:
//  - The current option does not contain a '=' or a ':'
//  - The current option requires a value (i.e. not a Option type of ':')
// The `name' used in the option format string does NOT include any leading
// option indicator, such as '-', '--', or '/'.  All three of these are
// permitted/required on any named option.
// Option bundling is permitted so long as:
//   - '-' is used to start the option group
//   - all of the bundled options are a single character
//   - at most one of the bundled options accepts a value, and the value
//     provided starts from the next character to the end of the string.
// This allows specifying '-a -b -c' as '-abc', and specifying '-D name=value'
// as '-Dname=value'.
// Option processing is disabled by specifying "--".  All options after "--"
// are returned by OptionSet.Parse() unchanged and unprocessed.
// Unprocessed options are returned from OptionSet.Parse().
// Examples:
//  int verbose = 0;
//  OptionSet p = new OptionSet ()
//    .Add ("v", v => ++verbose)
//    .Add ("name=|value=", v => Console.WriteLine (v));
//  p.Parse (new string[]{"-v", "--v", "/v", "-name=A", "/name", "B", "extra"});
// The above would parse the argument string array, and would invoke the
// lambda expression three times, setting `verbose' to 3 when complete.  
// It would also print out "A" and "B" to standard output.
// The returned array would contain the string "extra".
// C# 3.0 collection initializers are supported and encouraged:
//  var p = new OptionSet () {
//    { "h|?|help", v => ShowHelp () },
//  };
// System.ComponentModel.TypeConverter is also supported, allowing the use of
// custom data types in the callback type; TypeConverter.ConvertFromString()
// is used to convert the value option to an instance of the specified
// type:
//  var p = new OptionSet () {
//    { "foo=", (Foo f) => Console.WriteLine (f.ToString ()) },
//  };
// Random other tidbits:
//  - Boolean options (those w/o '=' or ':' in the option format string)
//    are explicitly enabled if they are followed with '+', and explicitly
//    disabled if they are followed with '-':
//      string a = null;
//      var p = new OptionSet () {
//        { "a", s => a = s },
//      };
//      p.Parse (new string[]{"-a"});   // sets v != null
//      p.Parse (new string[]{"-a+"});  // sets v != null
//      p.Parse (new string[]{"-a-"});  // sets v == null
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;

#if TEST
using NDesk.Options;
#endif

namespace NDesk.Options
{
    /// <summary>
    /// The option value collection.
    /// </summary>
    public class OptionValueCollection : IList, IList<string>
    {
        /// <summary>
        /// The c.
        /// </summary>
        private OptionContext c;

        /// <summary>
        /// The values.
        /// </summary>
        private List<string> values = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionValueCollection"/> class.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        internal OptionValueCollection(OptionContext c)
        {
            this.c = c;
        }

        /// <summary>
        /// Gets Count.
        /// </summary>
        public int Count
        {
            get
            {
                return values.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IsReadOnly.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether IList.IsFixedSize.
        /// </summary>
        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether ICollection.IsSynchronized.
        /// </summary>
        bool ICollection.IsSynchronized
        {
            get
            {
                return (values as ICollection).IsSynchronized;
            }
        }

        /// <summary>
        /// Gets ICollection.SyncRoot.
        /// </summary>
        object ICollection.SyncRoot
        {
            get
            {
                return (values as ICollection).SyncRoot;
            }
        }

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public string this[int index]
        {
            get
            {
                AssertValid(index);
                return index >= values.Count ? null : values[index];
            }

            set
            {
                values[index] = value;
            }
        }

        /// <summary>
        /// The i list.this.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                (values as IList)[index] = value;
            }
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Add(string item)
        {
            values.Add(item);
        }

        /// <summary>
        /// The clear.
        /// </summary>
        public void Clear()
        {
            values.Clear();
        }

        /// <summary>
        /// The contains.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The contains.
        /// </returns>
        public bool Contains(string item)
        {
            return values.Contains(item);
        }

        /// <summary>
        /// The copy to.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <param name="arrayIndex">
        /// The array index.
        /// </param>
        public void CopyTo(string[] array, int arrayIndex)
        {
            values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// The get enumerator.
        /// </summary>
        /// <returns>
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        /// <summary>
        /// The index of.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The index of.
        /// </returns>
        public int IndexOf(string item)
        {
            return values.IndexOf(item);
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        public void Insert(int index, string item)
        {
            values.Insert(index, item);
        }

        /// <summary>
        /// The remove.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The remove.
        /// </returns>
        public bool Remove(string item)
        {
            return values.Remove(item);
        }

        /// <summary>
        /// The remove at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        public void RemoveAt(int index)
        {
            values.RemoveAt(index);
        }

        /// <summary>
        /// The to array.
        /// </summary>
        /// <returns>
        /// </returns>
        public string[] ToArray()
        {
            return values.ToArray();
        }

        /// <summary>
        /// The to list.
        /// </summary>
        /// <returns>
        /// </returns>
        public List<string> ToList()
        {
            return new List<string>(values);
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The to string.
        /// </returns>
        public override string ToString()
        {
            return string.Join(", ", values.ToArray());
        }

        /// <summary>
        /// The i list. add.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The i list. add.
        /// </returns>
        int IList.Add(object value)
        {
            return (values as IList).Add(value);
        }

        /// <summary>
        /// The assert valid.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        /// <exception cref="OptionException">
        /// </exception>
        private void AssertValid(int index)
        {
            if (c.Option == null)
            {
                throw new InvalidOperationException("OptionContext.Option is null.");
            }

            if (index >= c.Option.MaxValueCount)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (c.Option.OptionValueType == OptionValueType.Required &&
                index >= values.Count)
            {
                throw new OptionException(
                    string.Format(
                        c.OptionSet.MessageLocalizer("Missing required value for option '{0}'."), c.OptionName), 
                    c.OptionName);
            }
        }

        /// <summary>
        /// The i list. contains.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The i list. contains.
        /// </returns>
        bool IList.Contains(object value)
        {
            return (values as IList).Contains(value);
        }

        /// <summary>
        /// The i collection. copy to.
        /// </summary>
        /// <param name="array">
        /// The array.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        void ICollection.CopyTo(Array array, int index)
        {
            (values as ICollection).CopyTo(array, index);
        }

        /// <summary>
        /// The i enumerable. get enumerator.
        /// </summary>
        /// <returns>
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return values.GetEnumerator();
        }

        /// <summary>
        /// The i list. index of.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The i list. index of.
        /// </returns>
        int IList.IndexOf(object value)
        {
            return (values as IList).IndexOf(value);
        }

        /// <summary>
        /// The i list. insert.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        void IList.Insert(int index, object value)
        {
            (values as IList).Insert(index, value);
        }

        /// <summary>
        /// The i list. remove.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        void IList.Remove(object value)
        {
            (values as IList).Remove(value);
        }

        /// <summary>
        /// The i list. remove at.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        void IList.RemoveAt(int index)
        {
            (values as IList).RemoveAt(index);
        }
    }

    /// <summary>
    /// The option context.
    /// </summary>
    public class OptionContext
    {
        /// <summary>
        /// The c.
        /// </summary>
        private OptionValueCollection c;

        /// <summary>
        /// The index.
        /// </summary>
        private int index;

        /// <summary>
        /// The name.
        /// </summary>
        private string name;

        /// <summary>
        /// The option.
        /// </summary>
        private Option option;

        /// <summary>
        /// The set.
        /// </summary>
        private OptionSet set;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionContext"/> class.
        /// </summary>
        /// <param name="set">
        /// The set.
        /// </param>
        public OptionContext(OptionSet set)
        {
            this.set = set;
            c = new OptionValueCollection(this);
        }

        /// <summary>
        /// Gets or sets Option.
        /// </summary>
        public Option Option
        {
            get
            {
                return option;
            }

            set
            {
                option = value;
            }
        }

        /// <summary>
        /// Gets or sets OptionIndex.
        /// </summary>
        public int OptionIndex
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
            }
        }

        /// <summary>
        /// Gets or sets OptionName.
        /// </summary>
        public string OptionName
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Gets OptionSet.
        /// </summary>
        public OptionSet OptionSet
        {
            get
            {
                return set;
            }
        }

        /// <summary>
        /// Gets OptionValues.
        /// </summary>
        public OptionValueCollection OptionValues
        {
            get
            {
                return c;
            }
        }
    }

    /// <summary>
    /// The option value type.
    /// </summary>
    public enum OptionValueType
    {
        /// <summary>
        /// The none.
        /// </summary>
        None, 

        /// <summary>
        /// The optional.
        /// </summary>
        Optional, 

        /// <summary>
        /// The required.
        /// </summary>
        Required, 
    }

    /// <summary>
    /// The option.
    /// </summary>
    public abstract class Option
    {
        /// <summary>
        /// The name terminator.
        /// </summary>
        private static readonly char[] NameTerminator = new[] {'=', ':'};

        /// <summary>
        /// The count.
        /// </summary>
        private int count;

        /// <summary>
        /// The description.
        /// </summary>
        private string description;

        /// <summary>
        /// The names.
        /// </summary>
        private string[] names;

        /// <summary>
        /// The prototype.
        /// </summary>
        private string prototype;

        /// <summary>
        /// The separators.
        /// </summary>
        private string[] separators;

        /// <summary>
        /// The type.
        /// </summary>
        private OptionValueType type;

        /// <summary>
        /// Initializes a new instance of the <see cref="Option"/> class.
        /// </summary>
        /// <param name="prototype">
        /// The prototype.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        protected Option(string prototype, string description)
            : this(prototype, description, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Option"/> class.
        /// </summary>
        /// <param name="prototype">
        /// The prototype.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="maxValueCount">
        /// The max value count.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="ArgumentException">
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        protected Option(string prototype, string description, int maxValueCount)
        {
            if (prototype == null)
            {
                throw new ArgumentNullException("prototype");
            }

            if (prototype.Length == 0)
            {
                throw new ArgumentException("Cannot be the empty string.", "prototype");
            }

            if (maxValueCount < 0)
            {
                throw new ArgumentOutOfRangeException("maxValueCount");
            }

            this.prototype = prototype;
            names = prototype.Split('|');
            this.description = description;
            count = maxValueCount;
            type = ParsePrototype();

            if (count == 0 && type != OptionValueType.None)
            {
                throw new ArgumentException(
                    "Cannot provide maxValueCount of 0 for OptionValueType.Required or " +
                    "OptionValueType.Optional.", 
                    "maxValueCount");
            }

            if (type == OptionValueType.None && maxValueCount > 1)
            {
                throw new ArgumentException(
                    string.Format("Cannot provide maxValueCount of {0} for OptionValueType.None.", maxValueCount), 
                    "maxValueCount");
            }

            if (Array.IndexOf(names, "<>") >= 0 &&
                ((names.Length == 1 && type != OptionValueType.None) ||
                 (names.Length > 1 && MaxValueCount > 1)))
            {
                throw new ArgumentException(
                    "The default option handler '<>' cannot require values.", 
                    "prototype");
            }
        }

        /// <summary>
        /// Gets Description.
        /// </summary>
        public string Description
        {
            get
            {
                return description;
            }
        }

        /// <summary>
        /// Gets MaxValueCount.
        /// </summary>
        public int MaxValueCount
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        /// Gets OptionValueType.
        /// </summary>
        public OptionValueType OptionValueType
        {
            get
            {
                return type;
            }
        }

        /// <summary>
        /// Gets Prototype.
        /// </summary>
        public string Prototype
        {
            get
            {
                return prototype;
            }
        }

        /// <summary>
        /// Gets Names.
        /// </summary>
        internal string[] Names
        {
            get
            {
                return names;
            }
        }

        /// <summary>
        /// Gets ValueSeparators.
        /// </summary>
        internal string[] ValueSeparators
        {
            get
            {
                return separators;
            }
        }

        /// <summary>
        /// The get names.
        /// </summary>
        /// <returns>
        /// </returns>
        public string[] GetNames()
        {
            return (string[])names.Clone();
        }

        /// <summary>
        /// The get value separators.
        /// </summary>
        /// <returns>
        /// </returns>
        public string[] GetValueSeparators()
        {
            if (separators == null)
            {
                return new string[0];
            }

            return (string[])separators.Clone();
        }

        /// <summary>
        /// The invoke.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        public void Invoke(OptionContext c)
        {
            OnParseComplete(c);
            c.OptionName = null;
            c.Option = null;
            c.OptionValues.Clear();
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The to string.
        /// </returns>
        public override string ToString()
        {
            return Prototype;
        }

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        /// <exception cref="OptionException">
        /// </exception>
        protected static T Parse<T>(string value, OptionContext c)
        {
            TypeConverter conv = TypeDescriptor.GetConverter(typeof(T));
            T t = default (T);
            try
            {
                if (value != null)
                {
                    t = (T)conv.ConvertFromString(value);
                }
            }
            catch (Exception e)
            {
                throw new OptionException(
                    string.Format(
                        c.OptionSet.MessageLocalizer("Could not convert string `{0}' to type {1} for option `{2}'."), 
                        value, 
                        typeof(T).Name, 
                        c.OptionName), 
                    c.OptionName, 
                    e);
            }

            return t;
        }

        /// <summary>
        /// The on parse complete.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        protected abstract void OnParseComplete(OptionContext c);

        /// <summary>
        /// The add separators.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="end">
        /// The end.
        /// </param>
        /// <param name="seps">
        /// The seps.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        private static void AddSeparators(string name, int end, ICollection<string> seps)
        {
            int start = -1;
            for (int i = end + 1; i < name.Length; ++i)
            {
                switch (name[i])
                {
                    case '{':
                        if (start != -1)
                        {
                            throw new ArgumentException(
                                string.Format("Ill-formed name/value separator found in \"{0}\".", name), 
                                "prototype");
                        }

                        start = i + 1;
                        break;
                    case '}':
                        if (start == -1)
                        {
                            throw new ArgumentException(
                                string.Format("Ill-formed name/value separator found in \"{0}\".", name), 
                                "prototype");
                        }

                        seps.Add(name.Substring(start, i - start));
                        start = -1;
                        break;
                    default:
                        if (start == -1)
                        {
                            seps.Add(name[i].ToString());
                        }

                        break;
                }
            }

            if (start != -1)
            {
                throw new ArgumentException(
                    string.Format("Ill-formed name/value separator found in \"{0}\".", name), 
                    "prototype");
            }
        }

        /// <summary>
        /// The parse prototype.
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentException">
        /// </exception>
        private OptionValueType ParsePrototype()
        {
            char type = '\0';
            var seps = new List<string>();
            for (int i = 0; i < names.Length; ++i)
            {
                string name = names[i];
                if (name.Length == 0)
                {
                    throw new ArgumentException("Empty option names are not supported.", "prototype");
                }

                int end = name.IndexOfAny(NameTerminator);
                if (end == -1)
                {
                    continue;
                }

                names[i] = name.Substring(0, end);
                if (type == '\0' || type == name[end])
                {
                    type = name[end];
                }
                else
                {
                    throw new ArgumentException(
                        string.Format("Conflicting option types: '{0}' vs. '{1}'.", type, name[end]), 
                        "prototype");
                }

                AddSeparators(name, end, seps);
            }

            if (type == '\0')
            {
                return OptionValueType.None;
            }

            if (count <= 1 && seps.Count != 0)
            {
                throw new ArgumentException(
                    string.Format("Cannot provide key/value separators for Options taking {0} value(s).", count), 
                    "prototype");
            }

            if (count > 1)
            {
                if (seps.Count == 0)
                {
                    separators = new[] {":", "="};
                }
                else if (seps.Count == 1 && seps[0].Length == 0)
                {
                    separators = null;
                }
                else
                {
                    separators = seps.ToArray();
                }
            }

            return type == '=' ? OptionValueType.Required : OptionValueType.Optional;
        }
    }

    /// <summary>
    /// The option exception.
    /// </summary>
    [Serializable]
    public class OptionException : Exception
    {
        /// <summary>
        /// The option.
        /// </summary>
        private string option;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class.
        /// </summary>
        public OptionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="optionName">
        /// The option name.
        /// </param>
        public OptionException(string message, string optionName)
            : base(message)
        {
            option = optionName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="optionName">
        /// The option name.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public OptionException(string message, string optionName, Exception innerException)
            : base(message, innerException)
        {
            option = optionName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected OptionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            option = info.GetString("OptionName");
        }

        /// <summary>
        /// Gets OptionName.
        /// </summary>
        public string OptionName
        {
            get
            {
                return option;
            }
        }

        /// <summary>
        /// The get object data.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        [SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("OptionName", option);
        }
    }

    /// <summary>
    /// The option action.
    /// </summary>
    /// <param name="key">
    /// The key.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <typeparam name="TKey">
    /// </typeparam>
    /// <typeparam name="TValue">
    /// </typeparam>
    public delegate void OptionAction<TKey, TValue>(TKey key, TValue value);

    /// <summary>
    /// The option set.
    /// </summary>
    public class OptionSet : KeyedCollection<string, Option>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionSet"/> class.
        /// </summary>
        public OptionSet()
            : this(delegate(string f) { return f; })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionSet"/> class.
        /// </summary>
        /// <param name="localizer">
        /// The localizer.
        /// </param>
        public OptionSet(Converter<string, string> localizer)
        {
            this.localizer = localizer;
        }

        /// <summary>
        /// The localizer.
        /// </summary>
        private Converter<string, string> localizer;

        /// <summary>
        /// Gets MessageLocalizer.
        /// </summary>
        public Converter<string, string> MessageLocalizer
        {
            get
            {
                return localizer;
            }
        }

        /// <summary>
        /// The get key for item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <returns>
        /// The get key for item.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        protected override string GetKeyForItem(Option item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("option");
            }

            if (item.Names != null && item.Names.Length > 0)
            {
                return item.Names[0];
            }

            // This should never happen, as it's invalid for Option to be
            // constructed w/o any names.
            throw new InvalidOperationException("Option has no names!");
        }

        /// <summary>
        /// The get option for name.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        [Obsolete("Use KeyedCollection.this[string]")]
        protected Option GetOptionForName(string option)
        {
            if (option == null)
            {
                throw new ArgumentNullException("option");
            }

            try
            {
                return base[option];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        /// <summary>
        /// The insert item.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        protected override void InsertItem(int index, Option item)
        {
            base.InsertItem(index, item);
            AddImpl(item);
        }

        /// <summary>
        /// The remove item.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            Option p = Items[index];

            // KeyedCollection.RemoveItem() handles the 0th item
            for (int i = 1; i < p.Names.Length; ++i)
            {
                Dictionary.Remove(p.Names[i]);
            }
        }

        /// <summary>
        /// The set item.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="item">
        /// The item.
        /// </param>
        protected override void SetItem(int index, Option item)
        {
            base.SetItem(index, item);
            RemoveItem(index);
            AddImpl(item);
        }

        /// <summary>
        /// The add impl.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        private void AddImpl(Option option)
        {
            if (option == null)
            {
                throw new ArgumentNullException("option");
            }

            var added = new List<string>(option.Names.Length);
            try
            {
                // KeyedCollection.InsertItem/SetItem handle the 0th name.
                for (int i = 1; i < option.Names.Length; ++i)
                {
                    Dictionary.Add(option.Names[i], option);
                    added.Add(option.Names[i]);
                }
            }
            catch (Exception)
            {
                foreach (string name in added)
                {
                    Dictionary.Remove(name);
                }

                throw;
            }
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <returns>
        /// </returns>
        public new OptionSet Add(Option option)
        {
            base.Add(option);
            return this;
        }

        /// <summary>
        /// The action option.
        /// </summary>
        private sealed class ActionOption : Option
        {
            /// <summary>
            /// The action.
            /// </summary>
            private Action<OptionValueCollection> action;

            /// <summary>
            /// Initializes a new instance of the <see cref="ActionOption"/> class.
            /// </summary>
            /// <param name="prototype">
            /// The prototype.
            /// </param>
            /// <param name="description">
            /// The description.
            /// </param>
            /// <param name="count">
            /// The count.
            /// </param>
            /// <param name="action">
            /// The action.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// </exception>
            public ActionOption(string prototype, string description, int count, Action<OptionValueCollection> action)
                : base(prototype, description, count)
            {
                if (action == null)
                {
                    throw new ArgumentNullException("action");
                }

                this.action = action;
            }

            /// <summary>
            /// The on parse complete.
            /// </summary>
            /// <param name="c">
            /// The c.
            /// </param>
            protected override void OnParseComplete(OptionContext c)
            {
                action(c.OptionValues);
            }
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="prototype">
        /// The prototype.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// </returns>
        public OptionSet Add(string prototype, Action<string> action)
        {
            return Add(prototype, null, action);
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="prototype">
        /// The prototype.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public OptionSet Add(string prototype, string description, Action<string> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            Option p = new ActionOption(
                prototype, 
                description, 
                1, 
                delegate(OptionValueCollection v) { action(v[0]); });
            base.Add(p);
            return this;
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="prototype">
        /// The prototype.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// </returns>
        public OptionSet Add(string prototype, OptionAction<string, string> action)
        {
            return Add(prototype, null, action);
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="prototype">
        /// The prototype.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public OptionSet Add(string prototype, string description, OptionAction<string, string> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            Option p = new ActionOption(
                prototype, 
                description, 
                2, 
                delegate(OptionValueCollection v) { action(v[0], v[1]); });
            base.Add(p);
            return this;
        }

        /// <summary>
        /// The action option.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        private sealed class ActionOption<T> : Option
        {
            /// <summary>
            /// The action.
            /// </summary>
            private Action<T> action;

            /// <summary>
            /// Initializes a new instance of the <see cref="ActionOption{T}"/> class.
            /// </summary>
            /// <param name="prototype">
            /// The prototype.
            /// </param>
            /// <param name="description">
            /// The description.
            /// </param>
            /// <param name="action">
            /// The action.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// </exception>
            public ActionOption(string prototype, string description, Action<T> action)
                : base(prototype, description, 1)
            {
                if (action == null)
                {
                    throw new ArgumentNullException("action");
                }

                this.action = action;
            }

            /// <summary>
            /// The on parse complete.
            /// </summary>
            /// <param name="c">
            /// The c.
            /// </param>
            protected override void OnParseComplete(OptionContext c)
            {
                action(Parse<T>(c.OptionValues[0], c));
            }
        }

        /// <summary>
        /// The action option.
        /// </summary>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <typeparam name="TValue">
        /// </typeparam>
        private sealed class ActionOption<TKey, TValue> : Option
        {
            /// <summary>
            /// The action.
            /// </summary>
            private OptionAction<TKey, TValue> action;

            /// <summary>
            /// Initializes a new instance of the <see cref="ActionOption{TKey,TValue}"/> class.
            /// </summary>
            /// <param name="prototype">
            /// The prototype.
            /// </param>
            /// <param name="description">
            /// The description.
            /// </param>
            /// <param name="action">
            /// The action.
            /// </param>
            /// <exception cref="ArgumentNullException">
            /// </exception>
            public ActionOption(string prototype, string description, OptionAction<TKey, TValue> action)
                : base(prototype, description, 2)
            {
                if (action == null)
                {
                    throw new ArgumentNullException("action");
                }

                this.action = action;
            }

            /// <summary>
            /// The on parse complete.
            /// </summary>
            /// <param name="c">
            /// The c.
            /// </param>
            protected override void OnParseComplete(OptionContext c)
            {
                action(
                    Parse<TKey>(c.OptionValues[0], c), 
                    Parse<TValue>(c.OptionValues[1], c));
            }
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="prototype">
        /// The prototype.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public OptionSet Add<T>(string prototype, Action<T> action)
        {
            return Add(prototype, null, action);
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="prototype">
        /// The prototype.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public OptionSet Add<T>(string prototype, string description, Action<T> action)
        {
            return Add(new ActionOption<T>(prototype, description, action));
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="prototype">
        /// The prototype.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <typeparam name="TValue">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public OptionSet Add<TKey, TValue>(string prototype, OptionAction<TKey, TValue> action)
        {
            return Add(prototype, null, action);
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="prototype">
        /// The prototype.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <typeparam name="TKey">
        /// </typeparam>
        /// <typeparam name="TValue">
        /// </typeparam>
        /// <returns>
        /// </returns>
        public OptionSet Add<TKey, TValue>(string prototype, string description, OptionAction<TKey, TValue> action)
        {
            return Add(new ActionOption<TKey, TValue>(prototype, description, action));
        }

        /// <summary>
        /// The create option context.
        /// </summary>
        /// <returns>
        /// </returns>
        protected virtual OptionContext CreateOptionContext()
        {
            return new OptionContext(this);
        }

		public List<string> Parse (IEnumerable<string> arguments)
		{
			bool process = true;
			OptionContext c = CreateOptionContext ();
			c.OptionIndex = -1;
			var def = GetOptionForName ("<>");
			var unprocessed = 
				from argument in arguments
				where ++c.OptionIndex >= 0 && (process || def != null)
					? process
						? argument == "--" 
							? (process = false)
							: !Parse (argument, c)
								? def != null 
									? Unprocessed (null, def, c, argument) 
									: true
								: false
						: def != null 
							? Unprocessed (null, def, c, argument)
							: true
					: true
				select argument;
			List<string> r = unprocessed.ToList ();
			if (c.Option != null)
				c.Option.Invoke (c);
			return r;
		}

        /// <summary>
        /// The unprocessed.
        /// </summary>
        /// <param name="extra">
        /// The extra.
        /// </param>
        /// <param name="def">
        /// The def.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <returns>
        /// The unprocessed.
        /// </returns>
        private static bool Unprocessed(ICollection<string> extra, Option def, OptionContext c, string argument)
        {
            if (def == null)
            {
                extra.Add(argument);
                return false;
            }

            c.OptionValues.Add(argument);
            c.Option = def;
            c.Option.Invoke(c);
            return false;
        }

        /// <summary>
        /// The value option.
        /// </summary>
        private readonly Regex ValueOption = new Regex(
            @"^(?<flag>--|-|/)(?<name>[^:=]+)((?<sep>[:=])(?<value>.*))?$");

        /// <summary>
        /// The get option parts.
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <param name="flag">
        /// The flag.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sep">
        /// The sep.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The get option parts.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        protected bool GetOptionParts(
            string argument, out string flag, out string name, out string sep, out string value)
        {
            if (argument == null)
            {
                throw new ArgumentNullException("argument");
            }

            flag = name = sep = value = null;
            Match m = ValueOption.Match(argument);
            if (!m.Success)
            {
                return false;
            }

            flag = m.Groups["flag"].Value;
            name = m.Groups["name"].Value;
            if (m.Groups["sep"].Success && m.Groups["value"].Success)
            {
                sep = m.Groups["sep"].Value;
                value = m.Groups["value"].Value;
            }

            return true;
        }

        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The parse.
        /// </returns>
        protected virtual bool Parse(string argument, OptionContext c)
        {
            if (c.Option != null)
            {
                ParseValue(argument, c);
                return true;
            }

            string f, n, s, v;
            if (!GetOptionParts(argument, out f, out n, out s, out v))
            {
                return false;
            }

            Option p;
            if (Contains(n))
            {
                p = this[n];
                c.OptionName = f + n;
                c.Option = p;
                switch (p.OptionValueType)
                {
                    case OptionValueType.None:
                        c.OptionValues.Add(n);
                        c.Option.Invoke(c);
                        break;
                    case OptionValueType.Optional:
                    case OptionValueType.Required:
                        ParseValue(v, c);
                        break;
                }

                return true;
            }

            // no match; is it a bool option?
            if (ParseBool(argument, n, c))
            {
                return true;
            }

            // is it a bundled option?
            if (ParseBundledValue(f, string.Concat(n + s + v), c))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// The parse value.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <exception cref="OptionException">
        /// </exception>
        private void ParseValue(string option, OptionContext c)
        {
            if (option != null)
            {
                foreach (string o in c.Option.ValueSeparators != null
                                         ? option.Split(c.Option.ValueSeparators, StringSplitOptions.None)
                                         : new[] {option})
                {
                    c.OptionValues.Add(o);
                }
            }

            if (c.OptionValues.Count == c.Option.MaxValueCount ||
                c.Option.OptionValueType == OptionValueType.Optional)
            {
                c.Option.Invoke(c);
            }
            else if (c.OptionValues.Count > c.Option.MaxValueCount)
            {
                throw new OptionException(
                    localizer(
                        string.Format(
                            "Error: Found {0} option values when expecting {1}.", 
                            c.OptionValues.Count, 
                            c.Option.MaxValueCount)), 
                    c.OptionName);
            }
        }

        /// <summary>
        /// The parse bool.
        /// </summary>
        /// <param name="option">
        /// The option.
        /// </param>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The parse bool.
        /// </returns>
        private bool ParseBool(string option, string n, OptionContext c)
        {
            Option p;
            string rn;
            if (n.Length >= 1 && (n[n.Length - 1] == '+' || n[n.Length - 1] == '-') &&
                Contains(rn = n.Substring(0, n.Length - 1)))
            {
                p = this[rn];
                string v = n[n.Length - 1] == '+' ? option : null;
                c.OptionName = option;
                c.Option = p;
                c.OptionValues.Add(v);
                p.Invoke(c);
                return true;
            }

            return false;
        }

        /// <summary>
        /// The parse bundled value.
        /// </summary>
        /// <param name="f">
        /// The f.
        /// </param>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <returns>
        /// The parse bundled value.
        /// </returns>
        /// <exception cref="OptionException">
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        private bool ParseBundledValue(string f, string n, OptionContext c)
        {
            if (f != "-")
            {
                return false;
            }

            for (int i = 0; i < n.Length; ++i)
            {
                Option p;
                string opt = f + n[i].ToString();
                string rn = n[i].ToString();
                if (!Contains(rn))
                {
                    if (i == 0)
                    {
                        return false;
                    }

                    throw new OptionException(
                        string.Format(
                            localizer(
                                "Cannot bundle unregistered option '{0}'."), 
                            opt), 
                        opt);
                }

                p = this[rn];
                switch (p.OptionValueType)
                {
                    case OptionValueType.None:
                        Invoke(c, opt, n, p);
                        break;
                    case OptionValueType.Optional:
                    case OptionValueType.Required:
                    {
                        string v = n.Substring(i + 1);
                        c.Option = p;
                        c.OptionName = opt;
                        ParseValue(v.Length != 0 ? v : null, c);
                        return true;
                    }

                    default:
                        throw new InvalidOperationException("Unknown OptionValueType: " + p.OptionValueType);
                }
            }

            return true;
        }

        /// <summary>
        /// The invoke.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="option">
        /// The option.
        /// </param>
        private static void Invoke(OptionContext c, string name, string value, Option option)
        {
            c.OptionName = name;
            c.Option = option;
            c.OptionValues.Add(value);
            option.Invoke(c);
        }

        /// <summary>
        /// The option width.
        /// </summary>
        private const int OptionWidth = 29;

        /// <summary>
        /// The write option descriptions.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        public void WriteOptionDescriptions(TextWriter o)
        {
            foreach (Option p in this)
            {
                int written = 0;
                if (!WriteOptionPrototype(o, p, ref written))
                {
                    continue;
                }

                if (written < OptionWidth)
                {
                    o.Write(new string(' ', OptionWidth - written));
                }
                else
                {
                    o.WriteLine();
                    o.Write(new string(' ', OptionWidth));
                }

                List<string> lines = GetLines(localizer(GetDescription(p.Description)));
                o.WriteLine(lines[0]);
                var prefix = new string(' ', OptionWidth + 2);
                for (int i = 1; i < lines.Count; ++i)
                {
                    o.Write(prefix);
                    o.WriteLine(lines[i]);
                }
            }
        }

        /// <summary>
        /// The write option prototype.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="p">
        /// The p.
        /// </param>
        /// <param name="written">
        /// The written.
        /// </param>
        /// <returns>
        /// The write option prototype.
        /// </returns>
        private bool WriteOptionPrototype(TextWriter o, Option p, ref int written)
        {
            string[] names = p.Names;

            int i = GetNextOptionIndex(names, 0);
            if (i == names.Length)
            {
                return false;
            }

            if (names[i].Length == 1)
            {
                Write(o, ref written, "  -");
                Write(o, ref written, names[0]);
            }
            else
            {
                Write(o, ref written, "      --");
                Write(o, ref written, names[0]);
            }

            for (i = GetNextOptionIndex(names, i + 1);
                 i < names.Length;
                 i = GetNextOptionIndex(names, i + 1))
            {
                Write(o, ref written, ", ");
                Write(o, ref written, names[i].Length == 1 ? "-" : "--");
                Write(o, ref written, names[i]);
            }

            if (p.OptionValueType == OptionValueType.Optional ||
                p.OptionValueType == OptionValueType.Required)
            {
                if (p.OptionValueType == OptionValueType.Optional)
                {
                    Write(o, ref written, localizer("["));
                }

                Write(o, ref written, localizer("=" + GetArgumentName(0, p.MaxValueCount, p.Description)));
                string sep = p.ValueSeparators != null && p.ValueSeparators.Length > 0
                                 ? p.ValueSeparators[0]
                                 : " ";
                for (int c = 1; c < p.MaxValueCount; ++c)
                {
                    Write(o, ref written, localizer(sep + GetArgumentName(c, p.MaxValueCount, p.Description)));
                }

                if (p.OptionValueType == OptionValueType.Optional)
                {
                    Write(o, ref written, localizer("]"));
                }
            }

            return true;
        }

        /// <summary>
        /// The get next option index.
        /// </summary>
        /// <param name="names">
        /// The names.
        /// </param>
        /// <param name="i">
        /// The i.
        /// </param>
        /// <returns>
        /// The get next option index.
        /// </returns>
        private static int GetNextOptionIndex(string[] names, int i)
        {
            while (i < names.Length && names[i] == "<>")
            {
                ++i;
            }

            return i;
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        /// <param name="n">
        /// The n.
        /// </param>
        /// <param name="s">
        /// The s.
        /// </param>
        private static void Write(TextWriter o, ref int n, string s)
        {
            n += s.Length;
            o.Write(s);
        }

        /// <summary>
        /// The get argument name.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <param name="maxIndex">
        /// The max index.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <returns>
        /// The get argument name.
        /// </returns>
        private static string GetArgumentName(int index, int maxIndex, string description)
        {
            if (description == null)
            {
                return maxIndex == 1 ? "VALUE" : "VALUE" + (index + 1);
            }

            string[] nameStart;
            if (maxIndex == 1)
            {
                nameStart = new[] {"{0:", "{"};
            }
            else
            {
                nameStart = new[] {"{" + index + ":"};
            }

            for (int i = 0; i < nameStart.Length; ++i)
            {
                int start, j = 0;
                do
                {
                    start = description.IndexOf(nameStart[i], j);
                }
 while (start >= 0 && j != 0 ? description[j++ - 1] == '{' : false);
                if (start == -1)
                {
                    continue;
                }

                int end = description.IndexOf("}", start);
                if (end == -1)
                {
                    continue;
                }

                return description.Substring(start + nameStart[i].Length, end - start - nameStart[i].Length);
            }

            return maxIndex == 1 ? "VALUE" : "VALUE" + (index + 1);
        }

        /// <summary>
        /// The get description.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <returns>
        /// The get description.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        private static string GetDescription(string description)
        {
            if (description == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(description.Length);
            int start = -1;
            for (int i = 0; i < description.Length; ++i)
            {
                switch (description[i])
                {
                    case '{':
                        if (i == start)
                        {
                            sb.Append('{');
                            start = -1;
                        }
                        else if (start < 0)
                        {
                            start = i + 1;
                        }

                        break;
                    case '}':
                        if (start < 0)
                        {
                            if ((i + 1) == description.Length || description[i + 1] != '}')
                            {
                                throw new InvalidOperationException("Invalid option description: " + description);
                            }

                            ++i;
                            sb.Append("}");
                        }
                        else
                        {
                            sb.Append(description.Substring(start, i - start));
                            start = -1;
                        }

                        break;
                    case ':':
                        if (start < 0)
                        {
                            goto default;
                        }

                        start = i + 1;
                        break;
                    default:
                        if (start < 0)
                        {
                            sb.Append(description[i]);
                        }

                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// The get lines.
        /// </summary>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <returns>
        /// </returns>
        private static List<string> GetLines(string description)
        {
            var lines = new List<string>();
            if (string.IsNullOrEmpty(description))
            {
                lines.Add(string.Empty);
                return lines;
            }

            int length = 80 - OptionWidth - 2;
            int start = 0, end;
            do
            {
                end = GetLineEnd(start, length, description);
                bool cont = false;
                if (end < description.Length)
                {
                    char c = description[end];
                    if (c == '-' || (char.IsWhiteSpace(c) && c != '\n'))
                    {
                        ++end;
                    }
                    else if (c != '\n')
                    {
                        cont = true;
                        --end;
                    }
                }

                lines.Add(description.Substring(start, end - start));
                if (cont)
                {
                    lines[lines.Count - 1] += "-";
                }

                start = end;
                if (start < description.Length && description[start] == '\n')
                {
                    ++start;
                }
            }
 while (end < description.Length);
            return lines;
        }

        /// <summary>
        /// The get line end.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <returns>
        /// The get line end.
        /// </returns>
        private static int GetLineEnd(int start, int length, string description)
        {
            int end = Math.Min(start + length, description.Length);
            int sep = -1;
            for (int i = start; i < end; ++i)
            {
                switch (description[i])
                {
                    case ' ':
                    case '\t':
                    case '\v':
                    case '-':
                    case ',':
                    case '.':
                    case ';':
                        sep = i;
                        break;
                    case '\n':
                        return i;
                }
            }

            if (sep == -1 || end == description.Length)
            {
                return end;
            }

            return sep;
        }
    }
}