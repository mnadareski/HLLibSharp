/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Packages
{
    public sealed class PackageAttribute
    {
        /// <summary>
        /// Currently assigned attribute type
        /// </summary>
        public AttributeType AttributeType { get; private set; }

        /// <summary>
        /// Attribute name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Flag for hexidecimal representation
        /// </summary>
        public bool Hexadecimal { get; private set; }

        /// <summary>
        /// Private backing field for other values
        /// </summary>
        private object value;

        #region Boolean

        /// <summary>
        /// Get the value from the attribute as a boolean
        /// </summary>
        public bool GetBoolean()
        {
            if (AttributeType != AttributeType.HL_ATTRIBUTE_BOOLEAN)
                return false;

            return (bool)this.value;
        }

        /// <summary>
        /// Set the value of the attribute as a boolean
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Boolean value to set</param>
        public void SetBoolean(string name, bool value)
        {
            AttributeType = AttributeType.HL_ATTRIBUTE_BOOLEAN;
            this.Name = name;
            this.value = value;
        }

        #endregion

        #region Integer

        /// <summary>
        /// Get the value from the attribute as an Int32
        /// </summary>
        public int GetInteger()
        {
            if (AttributeType != AttributeType.HL_ATTRIBUTE_INTEGER)
                return 0;

            return (int)this.value;
        }

        /// <summary>
        /// Set the value of the attribute as an Int32
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Int32 value to set</param>
        public void SetInteger(string name, int value)
        {
            AttributeType = AttributeType.HL_ATTRIBUTE_INTEGER;
            this.Name = name;
            this.value = value;
        }

        #endregion

        #region Unsigned Integer

        /// <summary>
        /// Get the value from the attribute as a UInt32
        /// </summary>
        public uint GetUnsignedInteger()
        {
            if (AttributeType != AttributeType.HL_ATTRIBUTE_UNSIGNED_INTEGER)
                return 0;

            return (uint)this.value;
        }

        /// <summary>
        /// Set the value of the attribute as a UInt32
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">UInt32 value to set</param>
        /// <param name="hexadecimal">Set if this a hexidecimal value</param>
        public void SetUnsignedInteger(string name, uint value, bool hexadecimal)
        {
            AttributeType = AttributeType.HL_ATTRIBUTE_UNSIGNED_INTEGER;
            this.Name = name;
            this.value = value;
            this.Hexadecimal = hexadecimal;
        }

        #endregion

        #region Float

        /// <summary>
        /// Get the value from the attribute as a Float
        /// </summary>
        public float GetFloat()
        {
            if (AttributeType != AttributeType.HL_ATTRIBUTE_FLOAT)
                return 0;

            return (uint)this.value;
        }

        /// <summary>
        /// Set the value of the attribute as a Float
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Int32 value to set</param>
        public void SetFloat(string name, float value)
        {
            AttributeType = AttributeType.HL_ATTRIBUTE_FLOAT;
            this.Name = name;
            this.value = value;
        }

        #endregion

        #region String

        /// <summary>
        /// Get the value from the attribute as a UInt32
        /// </summary>
        public string GetString()
        {
            if (AttributeType != AttributeType.HL_ATTRIBUTE_STRING)
                return string.Empty;

            return (string)this.value;
        }

        /// <summary>
        /// Set the value of the attribute as a UInt32
        /// </summary>
        /// <param name="name">Name of the attribute</param>
        /// <param name="value">Int32 value to set</param>
        public void SetString(string name, string value)
        {
            AttributeType = AttributeType.HL_ATTRIBUTE_STRING;
            this.Name = name;
            this.value = value;
        }

        #endregion
    }
}