export class NavigationConst {
  public static readonly Home: string = '/home';
  public static readonly Login: string = '/login';

  public static readonly MyLibrary: string = '/library';
  public static readonly BecomePublisher: string = '/become-publisher';
  public static readonly PublishStory: string = '/publish-story';
  public static readonly ReadingHistory: string = '/reading-history';
  public static readonly PublisherLibrary = (libraryId: string): string =>
    this.ByRouteParameter('library', libraryId);
  public static readonly PreviewStory = (storyId: string): string =>
    this.ByRouteParameter('preview-story', storyId);
  public static readonly ModifyStory = (storyId: string): string =>
    this.ByRouteParameter('modify-story', storyId);
  public static readonly ReadStory = (storyId: string): string =>
    this.ByRouteParameter('read-story', storyId);
  public static readonly SearchStory = (term: string): string =>
    this.ByRouteParameter('search-story', term);

  private static readonly ByRouteParameter = (route: string, routePath: string): string =>
    `/${route}/${routePath}`;
}
