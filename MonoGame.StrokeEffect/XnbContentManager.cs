using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace MonoGame;

public class XnbContentManager(MemoryStream xnbStream, GraphicsDevice graphicDevice)
    : ContentManager(new FakeServiceProvider(graphicDevice), "Content")
{
    class FakeGraphicsService(GraphicsDevice graphicDevice) : IGraphicsDeviceService
    {
        public GraphicsDevice GraphicsDevice { get; private set; } = graphicDevice;

#pragma warning disable 67
        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;
#pragma warning restore 67
    }

    class FakeServiceProvider(GraphicsDevice graphicDevice) : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IGraphicsDeviceService))
                return new FakeGraphicsService(graphicDevice);

            throw new NotImplementedException();
        }
    }

    public T Load<T>()
    {
        return base.Load<T>("MyMemoryStreamAsset");
    }

    protected override Stream OpenStream(string assetName)
    {
        return new MemoryStream(xnbStream.GetBuffer(), false);
    }
}