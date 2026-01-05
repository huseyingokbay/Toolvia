import CryptoJS from 'crypto-js'

export const hashMD5 = (input: string): string => {
  return CryptoJS.MD5(input).toString()
}

export const hashSHA1 = (input: string): string => {
  return CryptoJS.SHA1(input).toString()
}

export const hashSHA256 = (input: string): string => {
  return CryptoJS.SHA256(input).toString()
}

export const hashSHA512 = (input: string): string => {
  return CryptoJS.SHA512(input).toString()
}

export const hashSHA3 = (input: string, bits: 224 | 256 | 384 | 512 = 256): string => {
  return CryptoJS.SHA3(input, { outputLength: bits }).toString()
}

export const hashRIPEMD160 = (input: string): string => {
  return CryptoJS.RIPEMD160(input).toString()
}

export type HashAlgorithm = 'md5' | 'sha1' | 'sha256' | 'sha512' | 'sha3-256' | 'ripemd160'

export const hash = (input: string, algorithm: HashAlgorithm): string => {
  switch (algorithm) {
    case 'md5': return hashMD5(input)
    case 'sha1': return hashSHA1(input)
    case 'sha256': return hashSHA256(input)
    case 'sha512': return hashSHA512(input)
    case 'sha3-256': return hashSHA3(input, 256)
    case 'ripemd160': return hashRIPEMD160(input)
    default: return ''
  }
}
