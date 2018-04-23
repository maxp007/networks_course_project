#!/usr/bin/env python
# -*- coding: utf-8 -*-
# Cyclic Recundary Check 7 bits: 4 bits for data, 3 for checksum
# Code is written for using with IronPython in .Net environment.
import codecs
synd_table = {
    0b001: 0b0000001,
    0b010: 0b0000010,
    0b100: 0b0000100,
    0b011: 0b0001000,
    0b110: 0b0010000,
    0b111: 0b0100000,
    0b101: 0b1000000
}

# Input:  data int  to count crc poly = 1011000
# Output: crc

def getcrc(data_with_crc):
    temp = data_with_crc
    poly = 0b1011000
    shift_count = 0
    mask = 0b1000000
    while (1):
        temp = temp ^ poly
        if (temp & mask == mask):
            continue
        else:
            if (shift_count < 3):
                mask = mask >> 1
                poly = poly >> 1
                shift_count += 1
            else:
                break
    crc = temp
    return crc


# Input: Raw 4-bit bit uint
# Output Coded 7 bit bit uint

def encodefun(raw_data_value):
    coded_value = int
    crc = int
    crc = getcrc(raw_data_value << 3)
    coded_value = (raw_data_value << 3) + crc
    return coded_value


# Input: Coded 7 bit int
# Output: corrected value without crc

def decodefun(coded_value):
    syndrome = getcrc(coded_value)
    if (syndrome == 0):
        return coded_value >> 3
    else:
        return (synd_table.get(syndrome) ^ coded_value) >> 3


def encode_data(data):
    data = bytearray(data)
    encoded_bytes = bytearray(b"")
    for byte_chunk in data:
        nibble_1 = encodefun(byte_chunk >> 4)
        encoded_bytes.append(nibble_1)
        nibble_2 = encodefun(byte_chunk & 0b00001111)
        encoded_bytes.append(nibble_2)
    result_string = encoded_bytes.decode('ascii') 
    return result_string


def decode_data(data):
    data = bytearray(data)
    decoded_bytes = bytearray(b"")
    code = 0
    #Note, this condition must always be false
    if len(data) % 2 != 0:
        data.pop(len(data) - 1)

    for i in range(0, len(data), 2):
        try:
            first = ((data[i] & 0b01111000) << 1)
            second = ((data[i + 1] & 0b01111000) >> 3)
            code = first + second
            decoded_bytes.append(code)
        # Note, this exception must never occur
        except IndexError:
            decoded_bytes.pop(i)
            pass
    return decoded_bytes.decode('cp1251')  


# Should use console with unicode IO
if __name__ == '__main__':
    #data_to_encode = u"Съешь же ещё этих мягких французских булок, да выпей чаю. " \
           #u"The quick brown fox jumps over the lazy dog. "
          
    #print("message to encode")
    #print (data_to_encode)
    #data_to_encode_1251 = data_to_encode.encode('cp1251')
    #encoded_data = encode_data(data_to_encode_1251)
    #print("encoded message")
    #print (encoded_data)

    #encoded_data_ascii  = encoded_data.encode("ascii")
    #decoded_data = decode_data(encoded_data_ascii)
    #print("decoded message")
    #print (decoded_data)
    #input()
    pass
