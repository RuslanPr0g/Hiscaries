import { Pipe, PipeTransform } from '@angular/core';
import { ImageUrlSizes } from '@shared/models/image-url-sizes.model';

@Pipe({
  name: 'fallbackImage',
  standalone: true,
})
export class FallbackImagePipe implements PipeTransform {
  transform(images: ImageUrlSizes | string | null | undefined): string | null {
    if (!images) return null;

    if (typeof images === 'string') return images;

    return images.Small ?? images.Medium ?? images.Large ?? null;
  }
}
