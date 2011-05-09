Simply put, this provides an easy way to enable DEFLATE (or GZIP if you like sending extra bytes in your response) compression on IIS.

IIS's deflate compression mechanism only compresses responses with a status code of 200; this will compress any response.

Fork it, edit it, pull-request it, make fun of it, whatever.


TODO:

  - Add option to limit compression to responses that are over a certain length (like `minFileSizeForComp`)
  - Do we need to filter out responses that don't have content, like 204, 303, and 304?
  - Caching. 
  - Benchmark against the default compression mechanism. Is it even worth it?
  - Make it better