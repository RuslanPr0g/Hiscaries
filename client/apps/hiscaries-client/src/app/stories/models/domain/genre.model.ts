import { BaseIdModel } from '@shared/models/base-id.model';

export interface GenreModel extends BaseIdModel {
  Name: string;
  Description: string;
  ImagePreviewUrl: string;
}
