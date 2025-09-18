import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class SocialMediaIconMapperService {
  private _socialNetworks: Record<string, string> = {
    tiktok: 'pi-tiktok',
    youtube: 'pi-youtube',
    facebook: 'pi-facebook',
    instagram: 'pi-instagram',
    twitter: 'pi-twitter',
    linkedin: 'pi-linkedin',
    pinterest: 'pi-pinterest',
  };

  mapFromUrl(link: string): string {
    if (!link) {
      return '';
    }

    const hostname = new URL(link).hostname.toLowerCase();

    for (const key in this._socialNetworks) {
      if (hostname.includes(key)) {
        return this._socialNetworks[key];
      }
    }

    return 'pi-link';
  }
}
