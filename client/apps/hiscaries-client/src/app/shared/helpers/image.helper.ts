export const convertToBase64 = (byteArray: string, format = 'image/jpeg'): string => {
  return `data:${format};base64,${byteArray}`;
};
