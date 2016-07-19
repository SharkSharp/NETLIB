using System;

namespace NETLIB
{
    /// <summary>
    /// Is the basic unit of the network communication, in other words all the information that travels over 
    /// the network is converted in BasePack before transmission and is subsequently
    /// reassembled by the receiver. It simplifies operations with the network buffer and handle reading and writing.
    /// </summary>
    /// <example>
    /// This example shows how to create a new pack, put and get some fields from there. 
    /// <code>
    /// void Example()
    /// {
    ///     int d1 = 5;
    ///     int d2 = 6;
    ///     
    ///     int c1;
    ///     int c2;
    ///     
    ///     BasePack newPack = new BasePack();
    ///     newPack.ID = 10;
    ///     newPack.PutInt(d1);
    ///     newPack.PutInt(d2);
    ///     
    ///     c1 = newPack.GetInt();
    ///     c2 = newPack.GetInt();
    /// }
    /// </code>
    /// </example>
    public class BasePack
    {
        #region Variables

        /// <summary>
        /// Represent the maximum size of the network buffer. The pack buffer will never be bigger than this.
        /// <para>Used by <see cref="Publisher"/> to receive the network buffer</para>
        /// </summary>
        /// <seealso cref="Publisher"/>
        public static int packSize = 1500;

        /// <summary>
        /// Hold the pack information.
        /// <para>Used by <see cref="Publisher"/> to send the information</para>
        /// </summary>
        protected byte[] buffer;

        /// <summary>
        /// Stores the index to be used in the buffer for the next read.
        /// </summary>
        /// The read position is initialized to 1 because the first byte is used as a packet <see cref="ID"/>
        /// <seealso cref="ID"/>
        /// <seealso cref="IOPackHandler{TPack}.OnReceivedPackCall(TPack)"/>
        /// <seealso cref="Protocol{TPack}"/>
        /// <seealso cref="Protocol{TPack}.triggers"/>
        protected int readPosition = 1;

        /// <summary>
        /// Stores the index to be used in the buffer for the next write.
        /// </summary>
        /// The write position is initialized to 1 because the first byte is used as a packet <see cref="ID"/>
        /// <seealso cref="IOPackHandler{TPack}.OnReceivedPackCall(TPack)"/>
        protected int writePosition = 1;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize the inner buffer with packSize
        /// </summary>
        /// <seealso cref="packSize"/>
        public BasePack()
        {
            this.buffer = new byte[packSize];
        }

        /// <summary>
        /// Takes the <paramref name="basePack"/> inner buffer as its own inner beffer.
        /// The <see cref="readPosition"/> and the <see cref="writePosition"/> are not copied
        /// </summary>
        /// <param name="basePack">BasePack that will be copied</param>
        protected BasePack(BasePack basePack)
        {
            this.buffer = basePack.buffer;
        }

        /// <summary>
        /// Initialize the BasePack taking <paramref name="buffer"/> as your own inner buffer
        /// </summary>
        /// <param name="buffer">Source buffer</param>
        protected BasePack(byte[] buffer)
        {
            if (buffer.Length <= packSize)
            {
                this.buffer = buffer;
            }
            else
            {
                throw new ArgumentOutOfRangeException("buffer");
            }
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Used by <see cref="IOPackHandler{TPack}"/> to classify and redirect incoming packs to the proper handle function.
        /// <para>Refers to the first byte of the buffer.</para>
        /// </summary>
        /// <remarks>As with any application protocol it is required something to identify the kind of the package, thus,
        /// I set that the first byte in the buffer will be used for that purpose.</remarks>
        /// <seealso cref="IOPackHandler{TPack}.OnReceivedPackCall(TPack)"/>
        /// <seealso cref="Protocol{TPack}"/>
        /// <seealso cref="Protocol{TPack}.triggers"/>
        public virtual byte ID
        {
            get { return buffer[0]; }

            set { buffer[0] = value; }
        }

        /// <summary>
        /// Make the buffer's data public but deny the exchange of buffer reference.
        /// </summary>
        /// <param name="index">Index of the byte to be read.</param>
        /// <returns>A byte of the byffer indexed by index.</returns>
        /// <exception cref = "ArgumentOutOfRangeException">
        ///     When the index is larger than <see cref="buffer"/>
        /// </exception> 
        public virtual byte this[int index]
        {
            get { return buffer[index]; }

            set { buffer[index] = value; }
        }

        /// <summary>
        /// Length of the inner buffer.
        /// </summary>
        public virtual int Length
        {
            get { return buffer.Length; }
        }

        /// <summary>
        ///     Returns the inner buffer but deny the exchange of buffer reference.
        /// </summary>
        public virtual byte[] Buffer
        {
            get { return buffer; }
        }

        /// <summary>
        /// Gets and sets the readPosition.
        /// </summary>
        public virtual int ReadPosition
        {
            get { return readPosition; }
            set { readPosition = value; }
        }

        /// <summary>
        /// Gets and sets the writePosition.
        /// </summary>
        public virtual int WritePosition
        {
            get { return writePosition; }
            set { writePosition = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Copies a sequence of bytes to the pack and advances the current <see cref="writePosition"/> by the number of bytes copied.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the pack.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the pack.</param>
        /// <param name="count">The number of bytes to be copied.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="writePosition"/> plus <paramref name="count"/> is larger than the inner buffer length.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when the <paramref name="offset"/> is larger or equal than the buffer length 
        /// and when <paramref name="offset"/> plus <paramref name="count"/> is larger than the buffer size.
        /// </exception>
        public virtual void Write(byte[] buffer, int offset, int count)
        {
            if (writePosition + count > this.buffer.Length)
            {
                throw new IndexOutOfRangeException("Write position larger than buffer length.");
            }

            if (offset >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (offset + count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            for (int i = 0; i < count; i++)
            {
                this.buffer[writePosition] = buffer[offset + i];
                writePosition++;
            }
        }

        /// <summary>
        /// Copies a sequence of bytes from the pack and advances the current <see cref="readPosition"/> by the number of bytes copied.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies count bytes from pack to the buffer.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the pack.</param>
        /// <param name="count">The number of bytes to be copied.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="readPosition"/> plus <paramref name="count"/> is larger than the inner buffer length.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when the <paramref name="offset"/> is larger or equal than the buffer length
        /// and when <paramref name="offset"/> plus <paramref name="count"/> is larger than the buffer size.
        /// </exception>
        public virtual void Read(byte[] buffer, int offset, int count)
        {
            if (readPosition + count > this.buffer.Length)
            {
                throw new IndexOutOfRangeException("Read position larger than buffer length.");
            }

            if (offset >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("offset");
            }

            if (offset + count > buffer.Length)
            {
                throw new ArgumentOutOfRangeException("count");
            }

            for (int i = 0; i < count; i++)
            {
                buffer[offset + i] = this.buffer[readPosition];
                readPosition++;
            }
        }

        /// <remarks>
        /// First it reads an integer from the inner <see cref="buffer"/>, that refers to the lenght of the string.
        /// Then reads the chars one by one from the inner <see cref="buffer"/> and put them in a string.
        /// </remarks>
        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a string.
        /// </summary>
        /// <returns>String converted from the inner <see cref="buffer"/> started in <see cref="readPosition"/>.</returns>
        /// <exception cref="FormatException">
        /// Throws when the length of the string read from the inner <see cref="buffer"/> is 
        /// bigger than the remaining bytes in the inner <see cref="buffer"/>
        /// </exception>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="readPosition"/> is larger than the inner <see cref="buffer"/> length minus sizeof(int).
        /// </exception>
        public virtual string GetString()
        {
            if (readPosition > this.buffer.Length - sizeof(int))
            {
                throw new IndexOutOfRangeException("Read position larger than buffer length.");
            }

            string result = string.Empty;
            int size = BitConverter.ToInt32(buffer, readPosition);
            readPosition += sizeof(int);

            if (size > this.buffer.Length - readPosition)
            {
                throw new FormatException("The read length is larger than the remaining bytes!");
            }

            for (int i = readPosition; i < size + readPosition; i++)
            {
                result += (char)this.buffer[i];
            }
            readPosition += size + sizeof(int);
            return result;
        }


        /// <remarks>
        /// First it reads an integer from the inner <see cref="buffer"/>, that refers to the lenght of the string.
        /// Then reads the chars one by one from the <see cref="buffer"/> and put them in a string.
        /// </remarks>
        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a string.
        /// </summary>
        /// <param name="offset">Start position for the string conversion.</param>
        /// <returns>String converted from the inner <see cref="buffer"/> started in <paramref name="offset"/>.</returns>
        /// <exception cref="FormatException">
        /// Throws when the length of the string read from the inner <see cref="buffer"/> is 
        /// less than the remaining bytes in the inner <see cref="buffer"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> is larger than the inner <see cref="buffer"/> length minus sizeof(int).
        /// </exception>
        protected virtual string GetString(int offset)
        {
            if (offset > this.buffer.Length - sizeof(int))
            {
                throw new ArgumentOutOfRangeException("Offset larger than buffer length.");
            }

            string result = string.Empty;
            int size = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);

            if (size > this.Length - offset)
            {
                throw new FormatException("Entrada não se encaixa como uma string!");
            }

            for (int i = offset + sizeof(int); i < size + sizeof(int) + offset; i++)
            {
                result += (char)this.buffer[i];
            }
            return result;
        }

        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a int.
        /// </summary>
        /// <returns>Int converted from the inner <see cref="buffer"/> started in <see cref="readPosition"/>.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="readPosition"/> is larger than the inner <see cref="buffer"/> length minus sizeof(int).
        /// </exception>
        public virtual int GetInt()
        {
            if (readPosition > this.buffer.Length - sizeof(int))
            {
                throw new IndexOutOfRangeException("Read position larger than buffer length.");
            }

            int ret = BitConverter.ToInt32(buffer, readPosition);
            readPosition += sizeof(int);
            return ret;
        }

        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a int.
        /// </summary>
        /// <returns>Int converted from the inner <see cref="buffer"/> started in <paramref name="offset"/>.</returns>
        /// <param name="offset">Start position for the int conversion.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> is larger than the inner <see cref="buffer"/> length minus sizeof(int).
        /// </exception>
        protected virtual int GetInt(int offset)
        {
            if (offset > this.buffer.Length - sizeof(int))
            {
                throw new ArgumentOutOfRangeException("Offset larger than buffer length.");
            }

            return BitConverter.ToInt32(buffer, offset);
        }

        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a double.
        /// </summary>
        /// <returns>Double converted from the inner <see cref="buffer"/> started in <see cref="readPosition"/>.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="readPosition"/> is larger than the inner <see cref="buffer"/> length minus sizeof(double).
        /// </exception>
        public virtual double GetDouble()
        {
            if (readPosition > this.buffer.Length - sizeof(double))
            {
                throw new IndexOutOfRangeException("Read position larger than buffer length.");
            }

            double ret = BitConverter.ToDouble(buffer, readPosition);
            readPosition += sizeof(double);
            return ret;
        }

        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a double.
        /// </summary>
        /// <param name="offset">Start position for the double conversion.</param>
        /// <returns>Double converted from the inner <see cref="buffer"/> started in <paramref name="offset"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> is larger than the inner <see cref="buffer"/> length minus sizeof(double).
        /// </exception>
        protected virtual double GetDouble(int offset)
        {
            if (offset > this.buffer.Length - sizeof(double))
            {
                throw new ArgumentOutOfRangeException("Offset larger than buffer length.");
            }

            return BitConverter.ToDouble(buffer, offset);
        }

        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a float.
        /// </summary>
        /// <returns>Float converted from the inner <see cref="buffer"/> started in <see cref="readPosition"/>.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="readPosition"/> is larger than the inner <see cref="buffer"/> length minus sizeof(float).
        /// </exception>
        public virtual float GetFloat()
        {
            if (readPosition > this.buffer.Length - sizeof(float))
            {
                throw new IndexOutOfRangeException("Read position larger than buffer length.");
            }

            float ret = BitConverter.ToSingle(buffer, readPosition);
            readPosition += sizeof(float);
            return ret;
        }

        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a float.
        /// </summary>
        /// <param name="offset">Start position for the float conversion.</param>
        /// <returns>Float converted from the inner <see cref="buffer"/> started in <paramref name="offset"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> is larger than the inner <see cref="buffer"/> length minus sizeof(float).
        /// </exception>
        protected virtual float GetFloat(int offset)
        {
            if (offset > this.buffer.Length - sizeof(float))
            {
                throw new ArgumentOutOfRangeException("Offset larger than buffer length.");
            }
            return BitConverter.ToSingle(buffer, offset);
        }

        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a char.
        /// </summary>
        /// <returns>Char converted from the inner <see cref="buffer"/> started in <see cref="readPosition"/>.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="readPosition"/> is larger than the inner <see cref="buffer"/> length minus 1.
        /// </exception>
        public virtual char GetChar()
        {
            if (readPosition > this.buffer.Length - 1)
            {
                throw new IndexOutOfRangeException("Read position larger than buffer length.");
            }

            char ret = (char)buffer[readPosition];
            readPosition++;
            return ret;
        }

        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a char.
        /// </summary>
        /// <param name="offset">Start position for the char conversion.</param>
        /// <returns>Char converted from the inner <see cref="buffer"/> started in <paramref name="offset"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> is larger than the inner <see cref="buffer"/> length minus 1.
        /// </exception>
        protected virtual char GetChar(int offset)
        {
            if (offset > this.buffer.Length - 1)
            {
                throw new ArgumentOutOfRangeException("Offset larger than buffer length.");
            }

            return (char)buffer[offset];
        }

        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a byte.
        /// </summary>
        /// <returns>Byte converted from the inner <see cref="buffer"/> started in <see cref="readPosition"/>.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="readPosition"/> is larger than the inner <see cref="buffer"/> length minus 1.
        /// </exception>
        public virtual byte GetByte()
        {
            if (readPosition > this.buffer.Length - 1)
            {
                throw new IndexOutOfRangeException("Read position larger than buffer length.");
            }

            byte returnedByte = buffer[readPosition];
            readPosition++;
            return returnedByte;
        }

        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a byte.
        /// </summary>
        /// <param name="offset">Start position for the byte conversion.</param>
        /// <returns>Byte converted from the inner <see cref="buffer"/> started in <paramref name="offset"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> is larger than the inner <see cref="buffer"/> length minus 1.
        /// </exception>
        protected virtual byte GetByte(int offset)
        {
            if (offset > this.buffer.Length - 1)
            {
                throw new ArgumentOutOfRangeException("Offset larger than buffer length.");
            }

            return buffer[offset];
        }

        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a bool.
        /// </summary>
        /// <returns>Bool converted from the inner <see cref="buffer"/> started in <see cref="readPosition"/>.</returns>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="readPosition"/> is larger than the inner <see cref="buffer"/> length minus 1.
        /// </exception>
        public virtual bool GetBool()
        {
            if (readPosition > this.buffer.Length - 1)
            {
                throw new IndexOutOfRangeException("Read position larger than buffer length.");
            }

            bool ret = buffer[readPosition] == 1;
            readPosition++;
            return ret;
        }

        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a bool.
        /// </summary>
        /// <param name="offset">Start position for the bool conversion.</param>
        /// <returns>Bool converted from the inner <see cref="buffer"/> started in <paramref name="offset"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> is larger than the inner <see cref="buffer"/> length minus 1.
        /// </exception>
        protected virtual bool GetBool(int offset)
        {
            if (offset > this.buffer.Length - 1)
            {
                throw new ArgumentOutOfRangeException("Offset larger than buffer length.");
            }

            return buffer[offset] == 1;
        }

        /// <summary>
        /// Converts a part of the inner <see cref="buffer"/>, started in <see cref="readPosition"/>, to a <typeparamref name="CustomType"/>.
        /// </summary>
        /// <typeparam name="CustomType">
        /// Any class that implements <see cref="IPackable"/> and has a parameterless contructor.
        /// This method create a new intance of <typeparamref name="CustomType"/> and call 
        /// <see cref="IPackable.Unpack(BasePack)"/> to the class build itself from the data read from the pack.
        /// </typeparam>
        /// <returns>New instance of <typeparamref name="CustomType"/> created from the data in the pack using 
        /// <see cref="IPackable.Unpack(BasePack)"/> method.</returns>
        /// <seealso cref="IPackable"/>
        public virtual CustomType GetPackable<CustomType>() where CustomType : IPackable, new()
        {
            CustomType returnedCustomType = new CustomType();
            returnedCustomType.Unpack(this);
            return returnedCustomType;
        }

        /// <summary>
        /// Converts a string to a byte buffer and put in the inner <see cref="buffer"/>, started in <see cref="writePosition"/>.
        /// </summary>
        /// <param name="value"> String to be writed in the inner <see cref="buffer"/>.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="writePosition"/> plus <paramref name="value"/> length is larger or equal than the buffer length. 
        /// </exception>
        /// <remarks>
        /// First it put a integer in the inner <see cref="buffer"/>, that refers to the lenght of the string.
        /// Then puts the chars one by one from the string in the inner <see cref="buffer"/>.
        /// </remarks>
        public virtual void PutString(string value)
        {
            if (writePosition + value.Length >= buffer.Length)
            {
                throw new IndexOutOfRangeException("Write position larger than buffer length.");
            }

            int i;
            byte[] aux = BitConverter.GetBytes(value.Length);

            for (i = 0; i < sizeof(int); i++)
            {
                buffer[i + writePosition] = aux[i];
            }

            for (; i < value.Length + sizeof(int); i++)
            {
                buffer[i + writePosition] = (byte)value[i - sizeof(int)];
            }

            writePosition += value.Length + sizeof(int);
        }

        /// <summary>
        /// Converts a string to a byte buffer and put in the inner <see cref="buffer"/>, started in <paramref name="offset"/>.
        /// </summary>
        /// <param name="value">String to be writed in the inner <see cref="buffer"/>.</param>
        /// <param name="offset">Start position to put the string.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> plus <paramref name="value"/> length is larger or equal than the buffer length. 
        /// </exception>
        /// <remarks>
        /// First it put a integer in the inner <see cref="buffer"/>, that refers to the lenght of the string.
        /// Then puts the chars one by one from the string in the inner <see cref="buffer"/>.
        /// </remarks>
        protected virtual void PutString(string value, int offset)
        {
            if (offset + value.Length >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("Offset larger than buffer length.");
            }

            int i;
            byte[] aux = BitConverter.GetBytes(value.Length);

            for (i = 0; i < sizeof(int); i++)
            {
                buffer[i + offset] = aux[i];
            }

            for (; i < value.Length + sizeof(int); i++)
            {
                buffer[i + offset] = (byte)value[i - sizeof(int)];
            }
        }

        /// <summary>
        /// Converts a int to a byte buffer and put in the inner <see cref="buffer"/>, started in <see cref="writePosition"/>
        /// </summary>
        /// <param name="value">Int to be writed in the inner <see cref="buffer"/>.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="writePosition"/> plus the size of a int is larger or equal than the buffer length. 
        /// </exception>
        public virtual void PutInt(int value)
        {
            if (writePosition + sizeof(int) >= buffer.Length)
            {
                throw new IndexOutOfRangeException("Write position larger than buffer length.");
            }

            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + writePosition] = aux[i];
            }

            writePosition += aux.Length;
        }

        /// <summary>
        /// Converts a int to a byte buffer and put in the inner <see cref="buffer"/>, started in <paramref name="offset"/>.
        /// </summary>
        /// <param name="value">Int to be writed in the inner <see cref="buffer"/>.</param>
        /// <param name="offset">Start position to put the integer.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> plus the size of a int is larger or equal than the buffer length. 
        /// </exception>
        protected virtual void PutInt(int value, int offset)
        {
            if (offset + sizeof(int) >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("Offset larger than buffer length.");
            }

            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + offset] = aux[i];
            }
        }

        /// <summary>
        /// Converts a double to a byte buffer and put in the inner <see cref="buffer"/>, started in <see cref="writePosition"/>
        /// </summary>
        /// <param name="value">Double to be writed in the inner <see cref="buffer"/>.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="writePosition"/> plus the size of a double is larger or equal than the buffer length. 
        /// </exception>
        public virtual void PutDouble(double value)
        {
            if (writePosition + sizeof(double) >= buffer.Length)
            {
                throw new IndexOutOfRangeException("Write position larger than buffer length.");
            }

            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + writePosition] = aux[i];
            }

            writePosition += aux.Length;
        }

        /// <summary>
        /// Converts a double to a byte buffer and put in the inner <see cref="buffer"/>, started in <paramref name="offset"/>.
        /// </summary>
        /// <param name="value">Double to be writed in the inner <see cref="buffer"/>.</param>
        /// <param name="offset">Start position to put the double.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> plus the size of a double is larger or equal than the buffer length. 
        /// </exception>
        protected virtual void PutDouble(double value, int offset)
        {
            if (offset + sizeof(double) >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("Offset larger than buffer length.");
            }

            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + offset] = aux[i];
            }
        }

        /// <summary>
        /// Converts a float to a byte buffer and put in the inner <see cref="buffer"/>, started in <see cref="writePosition"/>
        /// </summary>
        /// <param name="value">Float to be writed in the inner <see cref="buffer"/>.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="writePosition"/> plus the size of a float is larger or equal than the buffer length. 
        /// </exception>
        public virtual void PutFloat(float value)
        {
            if (writePosition + sizeof(float) >= buffer.Length)
            {
                throw new IndexOutOfRangeException("Write position larger than buffer length.");
            }

            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + writePosition] = aux[i];
            }

            writePosition += aux.Length;
        }

        /// <summary>
        /// Converts a float to a byte buffer and put in the inner <see cref="buffer"/>, started in <paramref name="offset"/>.
        /// </summary>
        /// <param name="value">Float to be writed in the inner <see cref="buffer"/>.</param>
        /// <param name="offset">Start position to put the float.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> plus the size of a float is larger or equal than the buffer length. 
        /// </exception>
        protected virtual void PutFloat(float value, int offset)
        {
            if (offset + sizeof(float) >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("Offset larger than buffer length.");
            }

            int i;
            byte[] aux = BitConverter.GetBytes(value);

            for (i = 0; i < aux.Length; i++)
            {
                buffer[i + offset] = aux[i];
            }
        }

        /// <summary>
        /// Converts a char to a byte buffer and put in the inner <see cref="buffer"/>, started in <see cref="writePosition"/>
        /// </summary>
        /// <param name="value">Char to be writed in the inner <see cref="buffer"/>.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="writePosition"/> plus 1 is larger or equal than the buffer length. 
        /// </exception>
        public virtual void PutChar(char value)
        {
            if (writePosition + 1 >= buffer.Length)
            {
                throw new IndexOutOfRangeException("Write position larger than buffer length.");
            }

            buffer[writePosition] = (byte)value;
            writePosition++;
        }

        /// <summary>
        /// Converts a char to a byte buffer and put in the inner <see cref="buffer"/>, started in <see cref="writePosition"/>
        /// </summary>
        /// <param name="value">Char to be writed in the inner <see cref="buffer"/>.</param>
        /// <param name="offset">Start position to put the char.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> plus 1 is larger or equal than the buffer length. 
        /// </exception>
        protected virtual void PutChar(char value, int offset)
        {
            if (offset + 1 >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("Offset larger than buffer length.");
            }

            buffer[offset] = (byte)value;
        }

        /// <summary>
        /// Converts a byte to a byte buffer and put in the inner <see cref="buffer"/>, started in <see cref="writePosition"/>
        /// </summary>
        /// <param name="value">Byte to be writed in the inner <see cref="buffer"/>.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="writePosition"/> plus 1 is larger or equal than the buffer length. 
        /// </exception>
        public virtual void PutByte(byte value)
        {
            if (writePosition + 1 >= buffer.Length)
            {
                throw new IndexOutOfRangeException("Write position larger than buffer length.");
            }

            buffer[writePosition] = value;
            writePosition++;
        }

        /// <summary>
        /// Converts a byte to a byte buffer and put in the inner <see cref="buffer"/>, started in <see cref="writePosition"/>
        /// </summary>
        /// <param name="value">Byte to be writed in the inner <see cref="buffer"/>.</param>
        /// <param name="offset">Start position to put the byte.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> plus 1 is larger or equal than the buffer length. 
        /// </exception>
        protected virtual void PutByte(byte value, int offset)
        {
            if (offset + 1 >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("Offset larger than buffer length.");
            }

            buffer[offset] = value;
        }

        /// <summary>
        /// Converts a bool to a byte buffer and put in the inner <see cref="buffer"/>, started in <see cref="writePosition"/>
        /// </summary>
        /// <param name="value">Bool to be writed in the inner <see cref="buffer"/>.</param>
        /// <exception cref="IndexOutOfRangeException">
        /// Throws when <see cref="writePosition"/> plus 1 is larger or equal than the buffer length. 
        /// </exception>
        public virtual void PutBool(bool value)
        {
            if (writePosition + 1 >= buffer.Length)
            {
                throw new IndexOutOfRangeException("Write position larger than buffer length.");
            }

            buffer[writePosition] = (value) ? (byte)1 : (byte)0;
            writePosition++;
        }

        /// <summary>
        /// Converts a bool to a byte buffer and put in the inner <see cref="buffer"/>, started in <see cref="writePosition"/>
        /// </summary>
        /// <param name="value">Bool to be writed in the inner <see cref="buffer"/>.</param>
        /// <param name="offset">Start position to put the bool.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Throws when <paramref name="offset"/> plus 1 is larger or equal than the buffer length. 
        /// </exception>
        protected virtual void PutBool(bool value, int offset)
        {
            if (offset + 1 >= buffer.Length)
            {
                throw new ArgumentOutOfRangeException("Offset larger than buffer length.");
            }

            buffer[offset] = (value) ? (byte)1 : (byte)0;
        }

        /// <summary>
        /// Converts a <typeparamref name="CustomType"/> class that implemets Ipackable to a byte buffet and put in the inner <see cref="buffer"/>, started in <see cref="writePosition"/>
        /// </summary>
        /// <typeparam name="CustomType">
        /// Any class that implements <see cref="IPackable"/>.
        /// This method call <see cref="IPackable.Pack(BasePack)"/>, so that the class can package your data in this <see cref="BasePack"/>.
        /// </typeparam>
        /// <seealso cref="IPackable"/>
        public virtual void PutPackable<CustomType>(CustomType packable) where CustomType : IPackable
        {
            packable.Pack(this);
        }

        /// <summary>
        /// Makes a deep copy of the pack, by cloning the internal buffer for the new package.
        /// </summary>
        /// <returns>New BasePack cloned from this pack</returns>
        public virtual BasePack DeepCopy()
        {
            BasePack returnedBasePack = new BasePack();

            for (int i = 0; i < packSize; i++)
            {
                returnedBasePack[i] = this[i];
            }
            return returnedBasePack;
        }

        /// <summary>
        ///      Initialize and returns a new BasePack using <paramref name="buffer"/> as it's inner buffer.
        ///</summary>
        ///<param name="buffer">
        ///     The buffer used as inner buffer by the new pack.
        ///</param>
        ///<exception cref = "ArgumentOutOfRangeException">
        ///     Throws when <paramref name="buffer"/>.Length is diferent than <see cref="packSize"/>
        ///</exception>
        public static implicit operator BasePack(byte[] buffer)
        {
            if (buffer.Length == packSize)
            {
                return new BasePack(buffer);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Entrada não entra nos parâmetros do pacote!");
            }
        }

        #endregion
    }
}