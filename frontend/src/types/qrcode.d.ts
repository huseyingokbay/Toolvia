declare module 'qrcode' {
  interface QRCodeOptions {
    width?: number
    margin?: number
    color?: {
      dark?: string
      light?: string
    }
    errorCorrectionLevel?: 'L' | 'M' | 'Q' | 'H'
  }

  interface QRCodeToStringOptions extends QRCodeOptions {
    type?: 'svg' | 'utf8' | 'terminal'
  }

  export function toCanvas(
    canvas: HTMLCanvasElement,
    text: string,
    options?: QRCodeOptions
  ): Promise<void>

  export function toDataURL(
    text: string,
    options?: QRCodeOptions
  ): Promise<string>

  export function toString(
    text: string,
    options?: QRCodeToStringOptions
  ): Promise<string>

  export default {
    toCanvas,
    toDataURL,
    toString
  }
}
